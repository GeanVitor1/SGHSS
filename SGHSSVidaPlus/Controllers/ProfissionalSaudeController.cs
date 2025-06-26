using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams;
using SGHSSVidaPlus.Domain.Interfaces.Repository;
using SGHSSVidaPlus.Domain.Interfaces.Service;
using SGHSSVidaPlus.MVC.Additional;
using SGHSSVidaPlus.MVC.Models;
using System.Runtime.Intrinsics.X86;

namespace SGHSSVidaPlus.MVC.Controllers
{
    [Authorize]
    public class ProfissionalSaudeController : Controller
    {
        private readonly IProfissionalSaudeService _profissionalSaudeService;
        private readonly IProfissionalSaudeRepository _profissionalSaudeRepository;
        private readonly IMapper _mapper;

        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public ProfissionalSaudeController(IProfissionalSaudeService profissionalSaudeService, IProfissionalSaudeRepository profissionalSaudeRepository, IMapper mapper,
                                           ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            _profissionalSaudeService = profissionalSaudeService;
            _profissionalSaudeRepository = profissionalSaudeRepository;
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
                throw new ArgumentNullException($"Partial View '{viewName}' cannot be found by the View Engine.");
            }
            using (var writer = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model },
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    writer,
                    new HtmlHelperOptions()
                );
                await viewResult.View.RenderAsync(viewContext);
                return writer.GetStringBuilder().ToString();
            }
        }

        [ClaimsAuthorize("profissional_saude", "visualizar")]
        public async Task<ActionResult> Index() => View(await _profissionalSaudeRepository.BuscarProfissionais(new ProfissionalSaudeParams() { Ativo = true }));

        public async Task<IActionResult> BuscarProfissionais([FromQuery] ProfissionalSaudeParams parametros) => PartialView("_ProfissionaisSaude", await _profissionalSaudeRepository.BuscarProfissionais(parametros));

        [ClaimsAuthorize("profissional_saude", "incluir")]
        public ActionResult Incluir()
        {
            TempData.Remove("formacao-profissional");
            TempData.Remove("cursos-profissional");
            return View(new ProfissionalSaudeViewModel());
        }

        public IActionResult TelaNovaFormacaoAcademica() => PartialView("_NovaFormacaoAcademica");

        [HttpPost]
        public IActionResult IncluirFormacaoAcademica(FormacaoAcademicaProfissionalSaudeViewModel formacaoAdicionar)
        {
            if (string.IsNullOrWhiteSpace(formacaoAdicionar.Titulo))
                return Json(new { resultado = "falha", mensagem = "O título da formação é obrigatório" });

            var formacoes = TempData.Get<List<FormacaoAcademicaProfissionalSaudeViewModel>>("formacao-profissional") ?? new List<FormacaoAcademicaProfissionalSaudeViewModel>();
            if (formacoes.Any(f => f.Titulo == formacaoAdicionar.Titulo && f.InstituicaoEnsino == formacaoAdicionar.InstituicaoEnsino))
            {
                TempData.Put("formacao-profissional", formacoes);
                return Json(new { resultado = "falha", mensagem = "A formação informada já foi adicionada" });
            }

            formacoes.Add(formacaoAdicionar);
            TempData.Put("formacao-profissional", formacoes);

            var partialHtml = RenderPartialViewToString("_FormacaoAcademica", formacoes).Result;
            return Json(new { resultado = "sucesso", mensagem = "Formação acadêmica adicionada com sucesso.", partialHtml = partialHtml });
        }

        [HttpPost]
        public IActionResult RemoverFormacaoAcademica(string titulo, string instituicao)
        {
            var formacoes = TempData.Get<List<FormacaoAcademicaProfissionalSaudeViewModel>>("formacao-profissional") ?? new List<FormacaoAcademicaProfissionalSaudeViewModel>();
            formacoes.Remove(formacoes.FirstOrDefault(f => f.Titulo == titulo && f.InstituicaoEnsino == instituicao));
            TempData.Put("formacao-profissional", formacoes);

            var partialHtml = RenderPartialViewToString("_FormacaoAcademica", formacoes).Result;
            return Json(new { resultado = "sucesso", mensagem = "Formação removida com sucesso.", partialHtml = partialHtml });
        }

        public IActionResult TelaNovoCursoCertificacao() => PartialView("_NovoCursoCertificacao");

        [HttpPost]
        public IActionResult IncluirCursoCertificacao(CursosCertificacoesProfissionalSaudeViewModel cursoAdicionar)
        {
            if (string.IsNullOrWhiteSpace(cursoAdicionar.Titulo))
                return Json(new { resultado = "falha", mensagem = "O título do curso é obrigatório" });

            var cursos = TempData.Get<List<CursosCertificacoesProfissionalSaudeViewModel>>("cursos-profissional") ?? new List<CursosCertificacoesProfissionalSaudeViewModel>();
            if (cursos.Any(c => c.Titulo == cursoAdicionar.Titulo && c.InstituicaoEnsino == cursoAdicionar.InstituicaoEnsino && c.DuracaoHoras == cursoAdicionar.DuracaoHoras))
            {
                TempData.Put("cursos-profissional", cursos);
                return Json(new { resultado = "falha", mensagem = "O curso/certificação informado(a) já foi adicionado(a)" });
            }

            cursos.Add(cursoAdicionar);
            TempData.Put("cursos-profissional", cursos);

            var partialHtml = RenderPartialViewToString("_CursosCertificacoes", cursos).Result;
            return Json(new { resultado = "sucesso", mensagem = "Curso/Certificação adicionado(a) com sucesso.", partialHtml = partialHtml });
        }

        [HttpPost]
        public IActionResult RemoverCursoCertificacao(string titulo, double duracaoHoras, string instituicao)
        {
            var cursos = TempData.Get<List<CursosCertificacoesProfissionalSaudeViewModel>>("cursos-profissional") ?? new List<CursosCertificacoesProfissionalSaudeViewModel>();
            cursos.Remove(cursos.FirstOrDefault(c => c.Titulo == titulo && c.DuracaoHoras == duracaoHoras && c.InstituicaoEnsino == instituicao));
            TempData.Put("cursos-profissional", cursos);

            var partialHtml = RenderPartialViewToString("_CursosCertificacoes", cursos).Result;
            return Json(new { resultado = "sucesso", mensagem = "Curso/Certificação removido(a) com sucesso!", partialHtml = partialHtml });
        }

        [HttpPost]
        public async Task<JsonResult> Incluir(ProfissionalSaudeViewModel profissionalSaudeViewModel)
        {
            try
            {
                // CORREÇÃO AQUI: Ignorar validação para UsuarioInclusao e EspecialidadeCargo
                ModelState.Remove("UsuarioInclusao");
                ModelState.Remove("EspecialidadeCargo"); // Adicionado para EspecialidadeCargo

                if (!ModelState.IsValid)
                {
                    var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    TempData.Put("formacao-profissional", profissionalSaudeViewModel.Formacao);
                    TempData.Put("cursos-profissional", profissionalSaudeViewModel.Cursos);
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", erros) });
                }

                var profissional = _mapper.Map<ProfissionalSaude>(profissionalSaudeViewModel);
                profissional.UsuarioInclusao = User.Identity.Name;
                profissional.DataInclusao = DateTime.Now;
                profissional.Ativo = true;

                profissional.Formacao = _mapper.Map<List<FormacaoAcademicaProfissionalSaude>>(TempData.Get<List<FormacaoAcademicaProfissionalSaudeViewModel>>("formacao-profissional") ?? new List<FormacaoAcademicaProfissionalSaudeViewModel>()); // Corrigido o default para ViewModel
                profissional.Cursos = _mapper.Map<List<CursosCertificacoesProfissionalSaude>>(TempData.Get<List<CursosCertificacoesProfissionalSaudeViewModel>>("cursos-profissional") ?? new List<CursosCertificacoesProfissionalSaudeViewModel>()); // Corrigido o default para ViewModel

                var resultado = await _profissionalSaudeService.Incluir(profissional);

                if (!resultado.Valido)
                {
                    TempData.Put("formacao-profissional", _mapper.Map<List<FormacaoAcademicaProfissionalSaudeViewModel>>(profissional.Formacao));
                    TempData.Put("cursos-profissional", _mapper.Map<List<CursosCertificacoesProfissionalSaudeViewModel>>(profissional.Cursos));
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });
                }
                ;

                TempData["success"] = "Profissional de saúde incluído com sucesso!";
                return Json(new { resultado = "sucesso" });
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException as SqlException;
                if (innerException != null)
                {
                    if (innerException.Number == 2601 || innerException.Number == 2627)
                    {
                        return Json(new { resultado = "falha", mensagem = "Já existe um registro com os dados informados." });
                    }
                    else if (innerException.Number == 515)
                    {
                        return Json(new { resultado = "falha", mensagem = "Um campo obrigatório não foi preenchido. Verifique os dados do profissional, formação e cursos." });
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
                return Json(new { resultado = "falha", mensagem = "Ocorreu um erro inesperado ao incluir o profissional de saúde: " + e.Message });
            }
        }

        [ClaimsAuthorize("profissional_saude", "alterar")]
        public async Task<IActionResult> Editar(int id)
        {
            TempData.Remove("formacao-profissional");
            TempData.Remove("cursos-profissional");

            var profissional = (await _profissionalSaudeRepository.BuscarProfissionais(new ProfissionalSaudeParams()
            {
                Id = id,
                IncluirFormacaoCursos = true
            })).FirstOrDefault();

            if (profissional == null)
            {
                TempData["error"] = "Não foi possível localizar os dados do profissional de saúde.";
                return RedirectToAction("Index");
            }

            TempData.Put("formacao-profissional", _mapper.Map<List<FormacaoAcademicaProfissionalSaudeViewModel>>(profissional.Formacao));
            TempData.Put("cursos-profissional", _mapper.Map<List<CursosCertificacoesProfissionalSaudeViewModel>>(profissional.Cursos));

            return View(_mapper.Map<ProfissionalSaudeViewModel>(profissional));
        }

        [HttpPost]
        public async Task<JsonResult> Editar(ProfissionalSaudeViewModel profissionalSaudeViewModel)
        {
            try
            {
                ModelState.Remove("UsuarioInclusao");
                ModelState.Remove("EspecialidadeCargo");

                if (!ModelState.IsValid)
                {
                    var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    TempData.Put("formacao-profissional", profissionalSaudeViewModel.Formacao);
                    TempData.Put("cursos-profissional", profissionalSaudeViewModel.Cursos);
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", erros) });
                }

                var profissional = _mapper.Map<ProfissionalSaude>(profissionalSaudeViewModel);

                var formacaoViewModelFromTempData = TempData.Get<List<FormacaoAcademicaProfissionalSaudeViewModel>>("formacao-profissional") ?? new List<FormacaoAcademicaProfissionalSaudeViewModel>();
                profissional.Formacao = _mapper.Map<List<FormacaoAcademicaProfissionalSaude>>(formacaoViewModelFromTempData);

                var cursosViewModelFromTempData = TempData.Get<List<CursosCertificacoesProfissionalSaudeViewModel>>("cursos-profissional") ?? new List<CursosCertificacoesProfissionalSaudeViewModel>();
                profissional.Cursos = _mapper.Map<List<CursosCertificacoesProfissionalSaude>>(cursosViewModelFromTempData);

                var resultado = await _profissionalSaudeService.Editar(profissional);

                if (!resultado.Valido)
                {
                    TempData.Put("formacao-profissional", _mapper.Map<List<FormacaoAcademicaProfissionalSaudeViewModel>>(profissional.Formacao));
                    TempData.Put("cursos-profissional", _mapper.Map<List<CursosCertificacoesProfissionalSaudeViewModel>>(profissional.Cursos));
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });
                }
                ;

                TempData["success"] = "Profissional de saúde editado com sucesso!";
                return Json(new { resultado = "sucesso" });
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException as SqlException;
                if (innerException != null)
                {
                    if (innerException.Number == 2601 || innerException.Number == 2627)
                    {
                        return Json(new { resultado = "falha", mensagem = "Já existe um registro com os dados informados." });
                    }
                    else if (innerException.Number == 515)
                    {
                        return Json(new { resultado = "falha", mensagem = "Um campo obrigatório não foi preenchido. Verifique os dados do profissional, formação e cursos." });
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
                return Json(new { resultado = "falha", mensagem = "Ocorreu um erro inesperado ao editar o profissional de saúde: " + e.Message });
            }
        }


        [HttpPost]
        public async Task<JsonResult> AlterarStatus(int id)
        {
            try
            {
                var profissional = (await _profissionalSaudeRepository.ObterPorId(id));

                if (profissional == null)
                    return Json(new { resultado = "falha", mensagem = "Profissional não encontrado" });

                profissional.Ativo = !profissional.Ativo;

                var resultado = await _profissionalSaudeService.AlterarStatus(profissional);

                if (!resultado.Valido)
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });

                return Json(new { resultado = "sucesso" });
            }
            catch (Exception e)
            {
                return Json(new { resultado = "falha", mensagem = string.Join(" ", e.Message) });
            }
        }

        [ClaimsAuthorize("profissional_saude", "visualizar")]
        public async Task<IActionResult> Visualizar(int id)
        {
            var profissional = (await _profissionalSaudeRepository.BuscarProfissionais(new ProfissionalSaudeParams()
            {
                Id = id,
                IncluirFormacaoCursos = true
            })).FirstOrDefault();

            if (profissional == null)
            {
                TempData["error"] = "Profissional não encontrado.";
                return RedirectToAction("Index");
            }

            var viewModel = _mapper.Map<ProfissionalSaudeViewModel>(profissional);

            return View(viewModel); // você vai precisar criar a View Visualizar.cshtml também
        }
    }
}
