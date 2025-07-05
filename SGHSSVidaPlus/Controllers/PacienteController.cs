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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

// **CORREÇÃO AQUI: Adicione a diretiva using para o namespace onde ApplicationUser está definido**
using SGHSSVidaPlus.MVC.Data;

namespace SGHSSVidaPlus.MVC.Controllers
{
    public class PacientesController : Controller
    {
        public readonly IPacienteService _pacienteService;
        public readonly IPacienteRepository _pacienteRepository;
        private readonly IMapper _mapper;

        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<PacientesController> _logger;

        public PacientesController(
            IPacienteService pacienteService,
            IPacienteRepository pacienteRepository,
            IMapper mapper,
            ICompositeViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<PacientesController> logger)
        {
            _pacienteService = pacienteService;
            _pacienteRepository = pacienteRepository;
            _mapper = mapper;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;

            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
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

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPatient(RegisterPacienteViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userExists = await _userManager.FindByEmailAsync(model.Email);
                if (userExists != null)
                {
                    ModelState.AddModelError(string.Empty, "O e-mail informado já está cadastrado.");
                    TempData["ShowRegisterTab"] = true;
                    TempData["error"] = "O e-mail informado já está cadastrado.";
                    return RedirectToAction("Login", "Account", new { Area = "Identity" });
                }

                var cpfExists = (await _pacienteRepository.BuscarPacientes(new PacienteParams { CPF = model.CPF })).Any();
                if (cpfExists)
                {
                    ModelState.AddModelError(string.Empty, "O CPF informado já está cadastrado para outro paciente.");
                    TempData["ShowRegisterTab"] = true;
                    TempData["error"] = "O CPF informado já está cadastrado para outro paciente.";
                    return RedirectToAction("Login", "Account", new { Area = "Identity" });
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nome = model.Nome,
                    Admin = false,
                    EmailConfirmed = true,
                    Bloqueado = false
                };

                var result = await _userManager.CreateAsync(user, model.Senha);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Novo usuário paciente criado com sucesso.");

                    if (!await _roleManager.RoleExistsAsync("paciente"))
                    {
                        var pacienteRole = new IdentityRole("paciente");
                        await _roleManager.CreateAsync(pacienteRole);
                    }

                    var addToRoleResult = await _userManager.AddToRoleAsync(user, "paciente");
                    if (!addToRoleResult.Succeeded)
                    {
                        _logger.LogError($"Erro ao adicionar o usuário {user.UserName} à role 'paciente': {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                        ModelState.AddModelError(string.Empty, $"Erro interno: não foi possível atribuir a função de paciente. Tente novamente ou contate o suporte.");
                        await _userManager.DeleteAsync(user);
                        TempData["ShowRegisterTab"] = true;
                        TempData["error"] = "Erro interno ao atribuir função de paciente.";
                        return RedirectToAction("Login", "Account", new { Area = "Identity" });
                    }

                    await _userManager.AddClaimAsync(user, new Claim("Nome", user.Nome));
                    await _userManager.AddClaimAsync(user, new Claim("CPF", model.CPF));

                    var paciente = _mapper.Map<Paciente>(model);
                    // Certifique-se de que o nome da propriedade na entidade Paciente é 'UserId'
                    // ou 'ApplicationUserId' conforme sua definição. Baseado no seu Paciente.cs, é 'UserId'.
                    paciente.UserId = user.Id;
                    paciente.Ativo = true;
                    paciente.DataInclusao = DateTime.Now;
                    paciente.UsuarioInclusao = user.UserName;

                    paciente.Contatos.Add(new PacienteContato { Contato = model.Email, Tipo = "Email", IsWhatsApp = false });

                    if (!string.IsNullOrEmpty(model.TipoConsultaDesejada))
                    {
                        paciente.Historico.Add(new HistoricoPaciente
                        {
                            Titulo = "Interesse de Consulta (Auto-cadastro)",
                            Descricao = $"Paciente manifestou interesse em consulta: {model.TipoConsultaDesejada}",
                            DataEvento = DateTime.Now, // **CORREÇÃO AQUI: Era DateTime.DateTime.Now**
                            ProfissionalResponsavel = "Sistema (Auto-cadastro)"
                        });
                    }

                    var pacienteResult = await _pacienteService.Incluir(paciente);

                    if (!pacienteResult.Valido)
                    {
                        var errorMessage = string.Join(" ", pacienteResult.Mensagens);
                        _logger.LogError($"Erro ao salvar os dados do paciente: {errorMessage}");
                        ModelState.AddModelError(string.Empty, $"Erro ao salvar os dados do paciente: {errorMessage}");
                        await _userManager.DeleteAsync(user);
                        TempData["ShowRegisterTab"] = true;
                        TempData["error"] = errorMessage;
                        return RedirectToAction("Login", "Account", new { Area = "Identity" });
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("Paciente logado automaticamente após o cadastro.");

                    TempData["success"] = "Cadastro realizado com sucesso! Bem-vindo(a)!";

                    return RedirectToAction("Dashboard");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            TempData["ShowRegisterTab"] = true;
            TempData["error"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToAction("Login", "Account", new { Area = "Identity" });
        }


        [Authorize(Roles = "paciente")]
        public IActionResult Dashboard()
        {
            return View();
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
            return Json(new { resultado = "sucesso", mensagem = "Registro de histórico removido com sucesso.", partialHtml = partialHtml });
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
                ModelState.Remove("UsuarioInclusao");

                if (!ModelState.IsValid)
                {
                    var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    TempData.Put("contatos-paciente", pacienteViewModel.Contatos);
                    TempData.Put("historico-paciente", pacienteViewModel.Historico);
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", erros) });
                }

                var paciente = _mapper.Map<Paciente>(pacienteViewModel);
                paciente.UsuarioInclusao = User.Identity.Name;
                paciente.DataInclusao = DateTime.Now;
                paciente.Ativo = true;

                paciente.Contatos = _mapper.Map<List<PacienteContato>>(TempData.Get<List<PacienteContatoViewModel>>("contatos-paciente") ?? new List<PacienteContatoViewModel>());
                paciente.Historico = _mapper.Map<List<HistoricoPaciente>>(TempData.Get<List<HistoricoPacienteViewModel>>("historico-paciente") ?? new List<HistoricoPacienteViewModel>());

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
                ModelState.Remove("UsuarioInclusao");

                if (!ModelState.IsValid)
                {
                    var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    TempData.Put("contatos-paciente", pacienteViewModel.Contatos);
                    TempData.Put("historico-paciente", pacienteViewModel.Historico);
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", erros) });
                }

                var paciente = _mapper.Map<Paciente>(pacienteViewModel);

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