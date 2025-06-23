using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams;
using SGHSSVidaPlus.Domain.Interfaces.Repository;
using SGHSSVidaPlus.Domain.Interfaces.Service;
using SGHSSVidaPlus.MVC.Additional;
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
    [Authorize]
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

        [ClaimsAuthorize("paciente", "visualizar")]
        public async Task<ActionResult> Index() => View(await _pacienteRepository.BuscarPacientes(new PacienteParams() { Ativo = true }));

        public async Task<IActionResult> BuscarPacientes([FromQuery] PacienteParams parametros) => PartialView("_Pacientes", await _pacienteRepository.BuscarPacientes(parametros));

        public async Task<IActionResult> Visualizar(int id) => View(_mapper.Map<PacienteViewModel>((await _pacienteRepository.BuscarPacientes(new PacienteParams() { Id = id, IncluirContatosHistorico = true })).FirstOrDefault()));

        [ClaimsAuthorize("paciente", "incluir")]
        public ActionResult Incluir()
        {
            TempData.Remove("contatos-paciente");
            TempData.Remove("historico-paciente");
            return View(new PacienteViewModel());
        }

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

        [HttpPost]
        public async Task<JsonResult> Incluir(PacienteViewModel pacienteViewModel)
        {
            try
            {
                var paciente = _mapper.Map<Paciente>(pacienteViewModel);
                paciente.UsuarioInclusao = User.Identity.Name;
                paciente.DataInclusao = DateTime.Now;
                paciente.Ativo = true;

                paciente.Contatos = _mapper.Map<List<PacienteContato>>(TempData.Get<List<PacienteContatoViewModel>>("contatos-paciente"));
                paciente.Historico = _mapper.Map<List<HistoricoPaciente>>(TempData.Get<List<HistoricoPacienteViewModel>>("historico-paciente"));

                var resultado = await _pacienteService.Incluir(paciente);

                if (!resultado.Valido)
                {
                    TempData.Put("contatos-paciente", _mapper.Map<List<PacienteContatoViewModel>>(paciente.Contatos));
                    TempData.Put("historico-paciente", _mapper.Map<List<HistoricoPacienteViewModel>>(paciente.Historico));
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });
                }
                ;

                TempData["success"] = "Paciente Incluído com Sucesso!";
                return Json(new { resultado = "sucesso" });
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
                return RedirectToAction("Index");
            }

            TempData.Put("contatos-paciente", _mapper.Map<List<PacienteContatoViewModel>>(paciente.Contatos));
            TempData.Put("historico-paciente", _mapper.Map<List<HistoricoPacienteViewModel>>(paciente.Historico));

            return View(_mapper.Map<PacienteViewModel>(paciente));
        }

        [HttpPost]
        public async Task<JsonResult> Editar(PacienteViewModel pacienteViewModel)
        {
            try
            {
                var paciente = _mapper.Map<Paciente>(pacienteViewModel);

                // CORREÇÃO AQUI: Garante que as listas sejam não-nulas antes de passar para o serviço
                var contatosViewModelFromTempData = TempData.Get<List<PacienteContatoViewModel>>("contatos-paciente") ?? new List<PacienteContatoViewModel>();
                paciente.Contatos = _mapper.Map<List<PacienteContato>>(contatosViewModelFromTempData);

                var historicoViewModelFromTempData = TempData.Get<List<HistoricoPacienteViewModel>>("historico-paciente") ?? new List<HistoricoPacienteViewModel>();
                paciente.Historico = _mapper.Map<List<HistoricoPaciente>>(historicoViewModelFromTempData);

                var resultado = await _pacienteService.Editar(paciente);

                if (!resultado.Valido)
                {
                    TempData.Put("contatos-paciente", _mapper.Map<List<PacienteContatoViewModel>>(paciente.Contatos));
                    TempData.Put("historico-paciente", _mapper.Map<List<HistoricoPacienteViewModel>>(paciente.Historico));
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });
                }
                ;

                TempData["success"] = "Paciente Editado com Sucesso!";
                return Json(new { resultado = "sucesso" });
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

        public async Task<IActionResult> BuscarHistorico(int id)
        {
            var paciente = (await _pacienteRepository.BuscarPacientes(new PacienteParams() { Id = id, IncluirContatosHistorico = true })).FirstOrDefault();

            if (paciente == null)
                return Json(new { resultado = "falha", mensagem = "Paciente não encontrado" });

            return PartialView("_HistoricoDetalhes", _mapper.Map<PacienteViewModel>(paciente));
        }
    }
}