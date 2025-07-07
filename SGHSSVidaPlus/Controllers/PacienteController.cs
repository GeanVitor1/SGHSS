using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding; // Necessário para ModelStateDictionary
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams;
using SGHSSVidaPlus.Domain.Interfaces.Repository;
using SGHSSVidaPlus.Domain.Interfaces.Service;
using SGHSSVidaPlus.MVC.Additional; // Assumo que ClaimsAuthorize está aqui
using SGHSSVidaPlus.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace SGHSSVidaPlus.MVC.Controllers
{
    [Authorize] // Este authorize aqui significa que qualquer usuário LOGADO pode acessar as actions,
                // a menos que uma role específica seja definida na action.
    public class PacientesController : Controller
    {
        public readonly IPacienteService _pacienteService;
        public readonly IPacienteRepository _pacienteRepository;
        private readonly IMapper _mapper;

        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public PacientesController(
            IPacienteService pacienteService,
            IPacienteRepository pacienteRepository,
            IMapper mapper,
            ICompositeViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _pacienteService = pacienteService;
            _pacienteRepository = pacienteRepository;
            _mapper = mapper;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        private async Task<string> RenderPartialViewToString(string viewName, object model)
        {
            var actionContext = new ActionContext(HttpContext, RouteData, ControllerContext.ActionDescriptor);

            var viewResult = _viewEngine.FindView(actionContext, viewName, false);

            if (viewResult.View == null)
            {
                throw new ArgumentNullException($"{viewName} partial view cannot be found.");
            }

            using (var writer = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = model
                    },
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return writer.GetStringBuilder().ToString();
            }
        }

        [Authorize(Roles = "admin")] // Apenas admin pode ver a lista completa de pacientes
        public async Task<ActionResult> Index() => View(await _pacienteRepository.BuscarPacientes(new PacienteParams() { Ativo = true }));

        [Authorize(Roles = "admin")] // Apenas admin pode buscar pacientes por query
        public async Task<IActionResult> BuscarPacientes([FromQuery] PacienteParams parametros) => PartialView("_Pacientes", await _pacienteRepository.BuscarPacientes(parametros));

        [Authorize(Roles = "admin")] // Apenas admin pode visualizar qualquer paciente
        public async Task<IActionResult> Visualizar(int id) => View(_mapper.Map<PacienteViewModel>((await _pacienteRepository.BuscarPacientes(new PacienteParams() { Id = id, IncluirContatosHistorico = true })).FirstOrDefault()));

        [Authorize(Roles = "admin")] // Apenas admin pode abrir o formulário de inclusão (usando PacienteAdminRegisterViewModel)
        public ActionResult Incluir()
        {
            TempData.Remove("contatos-paciente");
            TempData.Remove("historico-paciente");
            // Usa o ViewModel específico para cadastro via Admin (sem campos obrigatórios de login)
            return View(new PacienteAdminRegisterViewModel());
        }

        // As ações de Contato e Histórico podem ser acessadas por qualquer usuário autorizado
        // (tanto admin quanto paciente que edite seu próprio perfil)
        public IActionResult TelaNovoContato() => PartialView("_NovoContato");

        [HttpGet]
        public IActionResult ObterContatosPaciente()
        {
            var contatos = TempData.Get<List<PacienteContatoViewModel>>("contatos-paciente") ?? new List<PacienteContatoViewModel>();
            return PartialView("_Contatos", contatos);
        }

        [HttpPost]
        public async Task<JsonResult> IncluirContato(PacienteContatoViewModel contatoAdicionar)
        {
            if (string.IsNullOrWhiteSpace(contatoAdicionar.Contato))
                return Json(new { resultado = "falha", mensagem = "O contato não pode ser vazio" });

            if (string.IsNullOrWhiteSpace(contatoAdicionar.Tipo))
                return Json(new { resultado = "falha", mensagem = "É preciso informar o tipo do contato" });

            var contatos = TempData.Get<List<PacienteContatoViewModel>>("contatos-paciente") ?? new List<PacienteContatoViewModel>();

            if (contatos.Any(c => c.Contato == contatoAdicionar.Contato && c.Tipo == contatoAdicionar.Tipo))
            {
                TempData.Put("contatos-paciente", contatos);
                return Json(new { resultado = "falha", mensagem = "O contato informado já foi adicionado" });
            }

            contatos.Add(contatoAdicionar);
            TempData.Put("contatos-paciente", contatos);

            var partialHtml = await RenderPartialViewToString("_Contatos", contatos);
            return Json(new { resultado = "sucesso", mensagem = "Contato adicionado com sucesso.", partialHtml = partialHtml });
        }

        [HttpPost]
        public async Task<JsonResult> RemoverContato(string contato, string tipo)
        {
            var contatos = TempData.Get<List<PacienteContatoViewModel>>("contatos-paciente") ?? new List<PacienteContatoViewModel>();
            contatos.Remove(contatos.FirstOrDefault(c => c.Contato == contato && c.Tipo == tipo));
            TempData.Put("contatos-paciente", contatos);

            var partialHtml = await RenderPartialViewToString("_Contatos", contatos);
            return Json(new { resultado = "sucesso", mensagem = "Contato removido com sucesso.", partialHtml = partialHtml });
        }

        public IActionResult TelaNovoHistorico() => PartialView("_NovoHistorico");

        [HttpGet]
        public IActionResult ObterHistoricoPaciente()
        {
            var historicos = TempData.Get<List<HistoricoPacienteViewModel>>("historico-paciente") ?? new List<HistoricoPacienteViewModel>();
            return PartialView("_Historico", historicos);
        }

        [HttpPost]
        public async Task<JsonResult> IncluirHistorico(HistoricoPacienteViewModel historicoAdicionar)
        {
            if (string.IsNullOrWhiteSpace(historicoAdicionar.Titulo))
                return Json(new { resultado = "falha", mensagem = "O título do registro é obrigatório" });

            var historicos = TempData.Get<List<HistoricoPacienteViewModel>>("historico-paciente") ?? new List<HistoricoPacienteViewModel>();
            if (historicos.Any(h => h.Titulo == historicoAdicionar.Titulo && h.DataEvento == historicoAdicionar.DataEvento))
            {
                TempData.Put("historico-paciente", historicos);
                return Json(new { resultado = "falha", mensagem = "O registro de histórico informado já foi adicionado" });
            }

            historicos.Add(historicoAdicionar);
            TempData.Put("historico-paciente", historicos);

            var partialHtml = await RenderPartialViewToString("_Historico", historicos);
            return Json(new { resultado = "sucesso", mensagem = "Registro de histórico adicionado com sucesso.", partialHtml = partialHtml });
        }

        [HttpPost]
        public async Task<JsonResult> RemoverHistorico(string titulo, DateTime? dataEvento)
        {
            var historicos = TempData.Get<List<HistoricoPacienteViewModel>>("historico-paciente") ?? new List<HistoricoPacienteViewModel>();
            historicos.Remove(historicos.FirstOrDefault(h => h.Titulo == titulo && h.DataEvento == dataEvento));
            TempData.Put("historico-paciente", historicos);

            var partialHtml = await RenderPartialViewToString("_Historico", historicos);
            return Json(new { resultado = "sucesso", mensagem = "Registro de histórico removido com sucesso.", partialHtml = partialHtml });
        }

        // Action para inclusão de paciente pelo ADMIN (usa PacienteAdminRegisterViewModel)
        [HttpPost]
        [Authorize(Roles = "admin")] // Apenas admin pode submeter este formulário
        public async Task<JsonResult> Incluir(PacienteAdminRegisterViewModel pacienteViewModel)
        {
            try
            {
                // Remover validações de Email, Senha e ConfirmarSenha se não forem fornecidas
                if (string.IsNullOrEmpty(pacienteViewModel.Email))
                {
                    ModelState.Remove(nameof(pacienteViewModel.Email));
                    ModelState.Remove(nameof(pacienteViewModel.Senha));
                    ModelState.Remove(nameof(pacienteViewModel.ConfirmarSenha));
                }
                else // Se o email foi fornecido, as senhas se tornam obrigatórias
                {
                    if (string.IsNullOrEmpty(pacienteViewModel.Senha))
                    {
                        ModelState.AddModelError(nameof(pacienteViewModel.Senha), "A senha é obrigatória se o e-mail for informado.");
                    }
                    if (string.IsNullOrEmpty(pacienteViewModel.ConfirmarSenha))
                    {
                        ModelState.AddModelError(nameof(pacienteViewModel.ConfirmarSenha), "A confirmação de senha é obrigatória se o e-mail for informado.");
                    }
                }

                // Ignora a validação para UsuarioInclusao e DataInclusao no ModelState,
                // pois serão preenchidos no backend
                ModelState.Remove("UsuarioInclusao");
                ModelState.Remove("DataInclusao");


                if (!ModelState.IsValid)
                {
                    var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    TempData.Put("contatos-paciente", pacienteViewModel.Contatos);
                    TempData.Put("historico-paciente", pacienteViewModel.Historico);
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", erros) });
                }

                var paciente = _mapper.Map<Paciente>(pacienteViewModel);
                paciente.UsuarioInclusao = User.Identity?.Name ?? "Admin (Erro Nome)"; // Garantia de nome
                paciente.DataInclusao = DateTime.Now;
                paciente.Ativo = true;
                paciente.ApplicationUserId = null; // Admin não cria ApplicationUser neste fluxo, então ele é nulo por padrão

                // Mapear listas de contatos e histórico do TempData
                paciente.Contatos = _mapper.Map<List<PacienteContato>>(TempData.Get<List<PacienteContatoViewModel>>("contatos-paciente") ?? new List<PacienteContatoViewModel>());
                paciente.Historico = _mapper.Map<List<HistoricoPaciente>>(TempData.Get<List<HistoricoPacienteViewModel>>("historico-paciente") ?? new List<HistoricoPacienteViewModel>());

                var resultado = await _pacienteService.Incluir(paciente);

                if (!resultado.Valido)
                {
                    TempData.Put("contatos-paciente", _mapper.Map<List<PacienteContatoViewModel>>(paciente.Contatos));
                    TempData.Put("historico-paciente", _mapper.Map<List<HistoricoPacienteViewModel>>(paciente.Historico));
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });
                }

                TempData["success"] = "Paciente Incluído com Sucesso!";
                return Json(new { resultado = "sucesso", redirectUrl = Url.Action("Index", "Pacientes") }); // Redireciona para Index após sucesso
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException as SqlException;
                if (innerException != null)
                {
                    if (innerException.Number == 2601 || innerException.Number == 2627)
                    {
                        return Json(new { resultado = "falha", mensagem = "Este CPF já existe no cadastro de pacientes." });
                    }
                    else if (innerException.Number == 515)
                    {
                        return Json(new { resultado = "falha", mensagem = "Um campo obrigatório não foi preenchido. Verifique os dados do paciente, contatos e histórico." });
                    }
                    else
                    {
                        return Json(new { resultado = "falha", mensagem = $"Erro no banco de dados: {innerException.Message}" });
                    }
                }
                return Json(new { resultado = "falha", mensagem = "Ocorreu um erro ao salvar os dados. Detalhes: " + ex.Message });
            }
            catch (Exception e)
            {
                return Json(new { resultado = "falha", mensagem = "Ocorreu um erro inesperado ao incluir o paciente: " + e.Message });
            }
        }

        // Action para abrir o formulário de edição de paciente.
        // Pode ser acessada por admin para qualquer paciente, ou pelo próprio paciente (se você tiver uma rota dedicada "Meu Perfil").
        // Para simplificar, mantemos como ClaimsAuthorize("paciente", "alterar") que geralmente o admin terá.
        [ClaimsAuthorize("paciente", "alterar")]
        public async Task<IActionResult> Editar(int id)
        {
            TempData.Remove("contatos-paciente");
            TempData.Remove("historico-paciente");

            var paciente = (await _pacienteRepository.BuscarPacientes(new PacienteParams()
            {
                Id = id,
                IncluirContatosHistorico = true
            })).FirstOrDefault();

            if (paciente == null)
            {
                TempData["error"] = "Não foi possível localizar os dados do paciente.";
                return RedirectToAction("Index"); // Ou para uma página de erro genérica
            }

            // Mapeia a entidade Paciente para o PacienteViewModel
            // IMPORTANTE: Isso significa que as propriedades Email, Senha, ConfirmarSenha do PacienteViewModel
            // serão preenchidas com os valores da entidade Paciente (que podem ser null/empty se não tiver login).
            var viewModel = _mapper.Map<PacienteViewModel>(paciente);

            // Se o paciente não tiver ApplicationUserId (ou seja, não tem login no Identity),
            // podemos limpar os campos de Email/Senha/ConfirmarSenha no ViewModel para que não apareçam no formulário
            // como requeridos, ou para evitar que campos nulos causem validação.
            // OU, você pode decidir exibir esses campos apenas se ApplicationUserId não for null.
            if (string.IsNullOrEmpty(viewModel.ApplicationUserId))
            {
                viewModel.Email = null;
                viewModel.Senha = null;
                viewModel.ConfirmarSenha = null;
            }


            TempData.Put("contatos-paciente", _mapper.Map<List<PacienteContatoViewModel>>(paciente.Contatos));
            TempData.Put("historico-paciente", _mapper.Map<List<HistoricoPacienteViewModel>>(paciente.Historico));

            return View(viewModel);
        }

        // Action para submeter a edição de paciente.
        // A lógica de ModelState.Remove precisa ser inteligente aqui.
        [HttpPost]
        [ClaimsAuthorize("paciente", "alterar")] // Apenas admin pode editar pacientes (ou paciente se for edição própria)
        public async Task<JsonResult> Editar(PacienteViewModel pacienteViewModel)
        {
            try
            {
                // NOVO: Adiciona a lógica para remover validações de Email/Senha/ConfirmarSenha
                // para o caso de edição, se o Email não for fornecido ou se o paciente não tiver ApplicationUserId.

                // 1. Remover UsuarioInclusao (já deve estar lá, mas bom manter)
                ModelState.Remove("UsuarioInclusao");
                ModelState.Remove("DataInclusao"); // Também pode ser removido, pois não é atualizado na edição


                // 2. Lógica para Email/Senha/ConfirmarSenha na EDIÇÃO:
                // Se o Email NÃO ESTÁ sendo fornecido (campo vazio) ou
                // se o paciente não tem um ApplicationUserId associado (não é um usuário Identity completo),
                // removemos as validações de email/senha/confirmação.
                // Isso permite salvar a edição sem exigir login/senha se não houver um associado.

                // Primeiro, obtenha o paciente existente do banco para verificar o ApplicationUserId
                var pacienteExistente = (await _pacienteRepository.BuscarPacientes(new PacienteParams { Id = pacienteViewModel.Id, IncluirContatosHistorico = true })).FirstOrDefault();

                if (pacienteExistente != null)
                {
                    // Se não há um ApplicationUser associado OU o email no ViewModel está vazio,
                    // removemos a obrigatoriedade de email e senha do ModelState.
                    if (string.IsNullOrEmpty(pacienteExistente.ApplicationUserId) || string.IsNullOrEmpty(pacienteViewModel.Email))
                    {
                        ModelState.Remove(nameof(pacienteViewModel.Email));
                        ModelState.Remove(nameof(pacienteViewModel.Senha));
                        ModelState.Remove(nameof(pacienteViewModel.ConfirmarSenha));
                    }
                    // Se existe um ApplicationUser associado E o email foi fornecido no ViewModel,
                    // mas a senha ou confirmação estão vazias, adiciona erro.
                    else if (!string.IsNullOrEmpty(pacienteExistente.ApplicationUserId) && !string.IsNullOrEmpty(pacienteViewModel.Email))
                    {
                        if (string.IsNullOrEmpty(pacienteViewModel.Senha))
                        {
                            ModelState.AddModelError(nameof(pacienteViewModel.Senha), "A senha é obrigatória se o e-mail for informado.");
                        }
                        if (string.IsNullOrEmpty(pacienteViewModel.ConfirmarSenha))
                        {
                            ModelState.AddModelError(nameof(pacienteViewModel.ConfirmarSenha), "A confirmação de senha é obrigatória se o e-mail for informado.");
                        }
                    }
                }
                else
                {
                    // Caso o paciente não seja encontrado para edição (o que deveria ser tratado antes)
                    return Json(new { resultado = "falha", mensagem = "Paciente não encontrado para edição." });
                }


                if (!ModelState.IsValid)
                {
                    var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    TempData.Put("contatos-paciente", pacienteViewModel.Contatos);
                    TempData.Put("historico-paciente", pacienteViewModel.Historico);
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", erros) });
                }

                // IMPORTANTE: Mapeie os dados do ViewModel para a entidade Paciente.
                // O .ReverseMap() no AutoMapper lida com isso.
                var paciente = _mapper.Map<Paciente>(pacienteViewModel);

                // Re-atribui as listas de contatos e histórico do TempData,
                // pois elas não são passadas diretamente pelo formulário principal.
                var contatosViewModelFromTempData = TempData.Get<List<PacienteContatoViewModel>>("contatos-paciente") ?? new List<PacienteContatoViewModel>();
                paciente.Contatos = _mapper.Map<List<PacienteContato>>(contatosViewModelFromTempData);

                var historicoViewModelFromTempData = TempData.Get<List<HistoricoPacienteViewModel>>("historico-paciente") ?? new List<HistoricoPacienteViewModel>();
                paciente.Historico = _mapper.Map<List<HistoricoPaciente>>(historicoViewModelFromTempData);

                // Certifique-se de que o ApplicationUserId do paciente existente não seja perdido se o ViewModel não o enviar.
                // A melhor prática seria carregar a entidade existente, atualizar suas propriedades, e salvar.
                // Ou, garantir que o ApplicationUserId seja um campo oculto no form de edição.
                paciente.ApplicationUserId = pacienteExistente.ApplicationUserId; // Preserva o ApplicationUserId existente


                var resultado = await _pacienteService.Editar(paciente);

                if (!resultado.Valido)
                {
                    TempData.Put("contatos-paciente", _mapper.Map<List<PacienteContatoViewModel>>(paciente.Contatos));
                    TempData.Put("historico-paciente", _mapper.Map<List<HistoricoPacienteViewModel>>(paciente.Historico));
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });
                }

                TempData["success"] = "Paciente Editado com Sucesso!";
                return Json(new { resultado = "sucesso", redirectUrl = Url.Action("Index", "Pacientes") });
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException as SqlException;
                if (innerException != null)
                {
                    if (innerException.Number == 2601 || innerException.Number == 2627)
                    {
                        return Json(new { resultado = "falha", mensagem = "Este CPF já existe no cadastro de pacientes." });
                    }
                    else if (innerException.Number == 515)
                    {
                        return Json(new { resultado = "falha", mensagem = "Um campo obrigatório não foi preenchido. Verifique os dados do paciente, contatos e histórico." });
                    }
                    else
                    {
                        return Json(new { resultado = "falha", mensagem = $"Erro no banco de dados: {innerException.Message}" });
                    }
                }
                return Json(new { resultado = "falha", mensagem = "Ocorreu um erro ao salvar os dados. Detalhes: " + ex.Message });
            }
            catch (Exception e)
            {
                return Json(new { resultado = "falha", mensagem = "Ocorreu um erro inesperado ao editar o paciente: " + e.Message });
            }
        }


        [HttpPost]
        [ClaimsAuthorize("paciente", "alterar")] // Ou uma policy mais específica, como "paciente.alterar_status"
        public async Task<JsonResult> AlterarStatus(int id)
        {
            try
            {
                var paciente = (await _pacienteRepository.ObterPorId(id));

                if (paciente == null)
                    return Json(new { resultado = "falha", mensagem = "Paciente não encontrado" });

                paciente.Ativo = !paciente.Ativo;

                var resultado = await _pacienteService.AlterarStatus(paciente);

                if (!resultado.Valido)
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });

                return Json(new { resultado = "sucesso" });
            }
            catch (Exception e)
            {
                return Json(new { resultado = "falha", mensagem = string.Join(" ", e.Message) });
            }
        }

        [Authorize] // Qualquer usuário logado pode buscar histórico de paciente, mas a View() deve ser protegida
        public async Task<IActionResult> BuscarHistorico(int id)
        {
            var paciente = (await _pacienteRepository.BuscarPacientes(new PacienteParams() { Id = id, IncluirContatosHistorico = true })).FirstOrDefault();

            if (paciente == null)
                return Json(new { resultado = "falha", mensagem = "Paciente não encontrado" });

            return PartialView("_HistoricoDetalhes", _mapper.Map<PacienteViewModel>(paciente));
        }
    }
}