using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGHSSVidaPlus.Domain.Entities; // Namespace atualizado
using SGHSSVidaPlus.Domain.ExtensionsParams; // Namespace atualizado
using SGHSSVidaPlus.Domain.Interfaces.Repository; // Namespace atualizado
using SGHSSVidaPlus.Domain.Interfaces.Service; // Namespace atualizado
using SGHSSVidaPlus.MVC.Additional; // Para TempDataAdditional e ClaimsAuthorizeAttribute
using SGHSSVidaPlus.MVC.Models; // Namespace atualizado
using System; // Para DateTime
using System.Collections.Generic; // Para List
using System.Linq; // Para LINQ
using System.Threading.Tasks; // Para Task

namespace SGHSSVidaPlus.MVC.Controllers // Namespace atualizado
{
    [Authorize]
    public class ProfissionalSaudeController : Controller // Nome da classe atualizado
    {
        private readonly IProfissionalSaudeService _profissionalSaudeService; // Serviço injetado
        private readonly IProfissionalSaudeRepository _profissionalSaudeRepository; // Repositório injetado
        private readonly IMapper _mapper;

        public ProfissionalSaudeController(IProfissionalSaudeService profissionalSaudeService, IProfissionalSaudeRepository profissionalSaudeRepository, IMapper mapper)
        {
            _profissionalSaudeService = profissionalSaudeService;
            _profissionalSaudeRepository = profissionalSaudeRepository;
            _mapper = mapper;
        }

        [ClaimsAuthorize("profissional_saude", "visualizar")] // Claim atualizada
        public async Task<ActionResult> Index() => View(await _profissionalSaudeRepository.BuscarProfissionais(new ProfissionalSaudeParams() { Ativo = true })); // Parâmetro atualizado

        public async Task<IActionResult> BuscarProfissionais([FromQuery] ProfissionalSaudeParams parametros) => PartialView("_ProfissionaisSaude", await _profissionalSaudeRepository.BuscarProfissionais(parametros)); // Parâmetros e view atualizados

        [ClaimsAuthorize("profissional_saude", "incluir")] // Claim atualizada
        public ActionResult Incluir()
        {
            // Limpa TempData para as listas do profissional
            TempData.Remove("formacao-profissional");
            TempData.Remove("cursos-profissional");

            return View();
        }

        public IActionResult TelaNovaFormacaoAcademica() => PartialView("_NovaFormacaoAcademica"); // Nova partial

        [HttpPost]
        public IActionResult IncluirFormacaoAcademica(FormacaoAcademicaProfissionalSaudeViewModel formacaoAdicionar) // ViewModel atualizado
        {
            if (string.IsNullOrWhiteSpace(formacaoAdicionar.Titulo))
                return Json(new { mensagem = "O título da formação é obrigatório" });

            var formacoes = TempData.Get<List<FormacaoAcademicaProfissionalSaudeViewModel>>("formacao-profissional") ?? new List<FormacaoAcademicaProfissionalSaudeViewModel>(); // Tipo atualizado

            if (formacoes.Any(f => f.Titulo == formacaoAdicionar.Titulo && f.InstituicaoEnsino == formacaoAdicionar.InstituicaoEnsino))
            {
                TempData.Put("formacao-profissional", formacoes);
                return Json(new { mensagem = "A formação informada já foi adicionada" });
            }

            formacoes.Add(formacaoAdicionar);
            TempData.Put("formacao-profissional", formacoes);
            return PartialView("_FormacaoAcademica", formacoes); // Partial atualizado
        }

        [HttpPost]
        public IActionResult RemoverFormacaoAcademica(string titulo, string instituicao) // Parâmetros atualizados
        {
            var formacoes = TempData.Get<List<FormacaoAcademicaProfissionalSaudeViewModel>>("formacao-profissional") ?? new List<FormacaoAcademicaProfissionalSaudeViewModel>(); // Tipo atualizado
            formacoes.Remove(formacoes.FirstOrDefault(f => f.Titulo == titulo && f.InstituicaoEnsino == instituicao)); // Comparação atualizada
            TempData.Put("formacao-profissional", formacoes);
            return PartialView("_FormacaoAcademica", formacoes); // Partial atualizado
        }

        public IActionResult TelaNovoCursoCertificacao() => PartialView("_NovoCursoCertificacao"); // Nova partial

        [HttpPost]
        public IActionResult IncluirCursoCertificacao(CursosCertificacoesProfissionalSaudeViewModel cursoAdicionar) // ViewModel atualizado
        {
            if (string.IsNullOrWhiteSpace(cursoAdicionar.Titulo))
                return Json(new { mensagem = "O título do curso é obrigatório" });

            var cursos = TempData.Get<List<CursosCertificacoesProfissionalSaudeViewModel>>("cursos-profissional") ?? new List<CursosCertificacoesProfissionalSaudeViewModel>(); // Tipo atualizado

            if (cursos.Any(c => c.Titulo == cursoAdicionar.Titulo && c.InstituicaoEnsino == cursoAdicionar.InstituicaoEnsino && c.DuracaoHoras == cursoAdicionar.DuracaoHoras))
            {
                TempData.Put("cursos-profissional", cursos);
                return Json(new { mensagem = "O curso/certificação informado(a) já foi adicionado(a)" });
            }

            cursos.Add(cursoAdicionar);
            TempData.Put("cursos-profissional", cursos);
            return PartialView("_CursosCertificacoes", cursos); // Partial atualizado
        }

        [HttpPost]
        public IActionResult RemoverCursoCertificacao(string titulo, double duracaoHoras, string instituicao) // Parâmetros atualizados
        {
            var cursos = TempData.Get<List<CursosCertificacoesProfissionalSaudeViewModel>>("cursos-profissional") ?? new List<CursosCertificacoesProfissionalSaudeViewModel>(); // Tipo atualizado
            cursos.Remove(cursos.FirstOrDefault(c => c.Titulo == titulo && c.DuracaoHoras == duracaoHoras && c.InstituicaoEnsino == instituicao)); // Comparação atualizada
            TempData.Put("cursos-profissional", cursos);
            return PartialView("_CursosCertificacoes", cursos); // Partial atualizado
        }


        [HttpPost]
        public async Task<JsonResult> Incluir(ProfissionalSaudeViewModel profissionalSaudeViewModel) // ViewModel atualizado
        {
            try
            {
                var profissional = _mapper.Map<ProfissionalSaude>(profissionalSaudeViewModel); // Entidade atualizada

                profissional.UsuarioInclusao = User.Identity.Name;
                profissional.DataInclusao = DateTime.Now;

                // Mapeia listas de ViewModels para Entidades
                profissional.Formacao = _mapper.Map<List<FormacaoAcademicaProfissionalSaude>>(TempData.Get<List<FormacaoAcademicaProfissionalSaudeViewModel>>("formacao-profissional")); // Tipo atualizado
                profissional.Cursos = _mapper.Map<List<CursosCertificacoesProfissionalSaude>>(TempData.Get<List<CursosCertificacoesProfissionalSaudeViewModel>>("cursos-profissional")); // Tipo atualizado

                var resultado = await _profissionalSaudeService.Incluir(profissional); // Serviço e entidade atualizados

                if (!resultado.Valido)
                {
                    // Se falhar, coloca as listas de volta no TempData para a View
                    TempData.Put("formacao-profissional", _mapper.Map<List<FormacaoAcademicaProfissionalSaudeViewModel>>(profissional.Formacao));
                    TempData.Put("cursos-profissional", _mapper.Map<List<CursosCertificacoesProfissionalSaudeViewModel>>(profissional.Cursos));

                    return Json(new
                    {
                        resultado = "falha",
                        mensagem = string.Join(" ", resultado.Mensagens)
                    });
                }

                TempData["success"] = "Profissional de Saúde Incluído com Sucesso!";
                return Json(new { resultado = "sucesso" });
            }
            catch (Exception e)
            {
                return Json(new { resultado = "falha", mensagem = string.Join(" ", e.Message) });
            }
        }


        [ClaimsAuthorize("profissional_saude", "alterar")] // Claim atualizada
        public async Task<IActionResult> Editar(int id)
        {
            // Limpa TempData antes de carregar novos dados
            TempData.Remove("formacao-profissional");
            TempData.Remove("cursos-profissional");

            var profissional = (await _profissionalSaudeRepository.BuscarProfissionais(new ProfissionalSaudeParams()
            {
                Id = id,
                IncluirFormacaoCursos = true // Parâmetro atualizado
            })).FirstOrDefault();

            if (profissional == null)
            {
                TempData["error"] = "Não foi possível localizar os dados do profissional de saúde.";
                return RedirectToAction("Index");
            }

            // Mapeia listas de Entidades para ViewModels antes de colocar no TempData
            TempData.Put("formacao-profissional", _mapper.Map<List<FormacaoAcademicaProfissionalSaudeViewModel>>(profissional.Formacao));
            TempData.Put("cursos-profissional", _mapper.Map<List<CursosCertificacoesProfissionalSaudeViewModel>>(profissional.Cursos));

            return View(_mapper.Map<ProfissionalSaudeViewModel>(profissional)); // ViewModel atualizado
        }

        [HttpPost]
        public async Task<JsonResult> Editar(ProfissionalSaudeViewModel profissionalSaudeViewModel) // ViewModel atualizado
        {
            try
            {
                var profissional = _mapper.Map<ProfissionalSaude>(profissionalSaudeViewModel); // Entidade atualizada

                // Mapeia listas de ViewModels para Entidades
                profissional.Formacao = _mapper.Map<List<FormacaoAcademicaProfissionalSaude>>(TempData.Get<List<FormacaoAcademicaProfissionalSaudeViewModel>>("formacao-profissional"));
                profissional.Cursos = _mapper.Map<List<CursosCertificacoesProfissionalSaude>>(TempData.Get<List<CursosCertificacoesProfissionalSaudeViewModel>>("cursos-profissional"));

                var resultado = await _profissionalSaudeService.Editar(profissional); // Serviço e entidade atualizados

                if (!resultado.Valido)
                {
                    // Se falhar, coloca as listas de volta no TempData para a View
                    TempData.Put("formacao-profissional", _mapper.Map<List<FormacaoAcademicaProfissionalSaudeViewModel>>(profissional.Formacao));
                    TempData.Put("cursos-profissional", _mapper.Map<List<CursosCertificacoesProfissionalSaudeViewModel>>(profissional.Cursos));

                    return Json(new
                    {
                        resultado = "falha",
                        mensagem = string.Join(" ", resultado.Mensagens)
                    });
                }
                ;

                TempData["success"] = "Profissional de Saúde Editado com Sucesso!";
                return Json(new { resultado = "sucesso" });
            }
            catch (Exception e)
            {
                return Json(new { resultado = "falha", mensagem = string.Join(" ", e.Message) });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AlterarStatus(int id)
        {
            try
            {
                var profissional = (await _profissionalSaudeRepository.ObterPorId(id)); // Entidade atualizada

                if (profissional == null)
                    return Json(new { resultado = "falha", mensagem = "Profissional de saúde não encontrado" });

                profissional.Ativo = !profissional.Ativo; // Inverte o status atual

                var resultado = await _profissionalSaudeService.AlterarStatus(profissional); // Serviço e entidade atualizados

                if (!resultado.Valido)
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });

                return Json(new { resultado = "sucesso" });
            }
            catch (Exception e)
            {
                return Json(new { resultado = "falha", mensagem = string.Join(" ", e.Message) });
            }
        }
    }
}