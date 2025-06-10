using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Selecao.Domain.Entities;
using Selecao.Domain.Interfaces.Repository;
using Selecao.Domain.Interfaces.Service;
using Selecao.MVC.additional;
using Selecao.MVC.Additional;
using Selecao.MVC.Migrations;
using Selecao.MVC.Models;

namespace Selecao.MVC.Controllers
{

    [Authorize]
    public class CandidatosController : Controller
    {

        public readonly ICandidatosService _candidatosService;
        public readonly ICandidatosRepository _candidatosRepository;
        private readonly IMapper _mapper;

        public CandidatosController(ICandidatosService candidatosService, ICandidatosRepository candidatosRepository, IMapper mapper)
        {
            _candidatosService = candidatosService;
            _candidatosRepository = candidatosRepository;
            _mapper = mapper;
        }

        [ClaimsAuthorize("candidato", "visualizar")]
        public async Task<ActionResult> Index() => View(await _candidatosRepository.BuscarCandidatos(new CandidatoParams() { Status = "1"}));

        public async Task<IActionResult> BuscarCandidatos([FromQuery] CandidatoParams parametros) => PartialView("_Candidatos", await _candidatosRepository.BuscarCandidatos(parametros));

        public async Task<IActionResult> Visualizar(int id) => View(_mapper.Map<CandidatoViewModel>((await _candidatosRepository.BuscarCandidatos(new CandidatoParams() { Id = id, InfoExtras = true })).FirstOrDefault()));

        [ClaimsAuthorize("candidato", "incluir")]
        public ActionResult Incluir()
        {
            TempData.Remove("contatos-candidato");
            TempData.Remove("formacoes-candidato");
            TempData.Remove("cursos-candidato");
            TempData.Remove("experiencias-candidato");
            return View();
        }

        public IActionResult TelaNovoContato() => PartialView("_NovoContato");

        [HttpPost]
        public IActionResult IncluirContato(CandidatoContato contatoAdicionar)
        {

            if (string.IsNullOrWhiteSpace(contatoAdicionar.Contato))
                return Json(new { mensagem = "O contato não pode ser vazio" });

            if (string.IsNullOrWhiteSpace(contatoAdicionar.Tipo))
                return Json(new { mensagem = "É preciso informar o tipo do contato" });

            var contatos = TempData.Get<List<CandidatoContato>>("contatos-candidato") ?? new List<CandidatoContato>();

            if (contatos.Any(c => c.Contato == contatoAdicionar.Contato && c.Tipo == contatoAdicionar.Tipo))
            {
                TempData.Put("contatos-candidato", contatos);
                return Json(new { mensagem = "O contato informado já foi adicionado" });
            }

            contatos.Add(contatoAdicionar);
            TempData.Put("contatos-candidato", contatos);
            return PartialView("_Contatos", contatos);
        }

        [HttpPost]
        public IActionResult RemoverContato(string contato, string tipo)
        {
            var contatos = TempData.Get<List<CandidatoContato>>("contatos-candidato") ?? new List<CandidatoContato>();
            contatos.Remove(contatos.FirstOrDefault(c => c.Contato == contato && c.Tipo == tipo));
            TempData.Put("contatos-candidato", contatos);
            return PartialView("_Contatos", contatos);
        }

        public IActionResult TelaNovaFormacao() => PartialView("_NovaFormacao");


        [HttpPost]
        public IActionResult IncluirFormacao(CandidatoFormacao formacao)
        {
            var formacoes = TempData.Get<List<CandidatoFormacao>>("formacoes-candidato") ?? new List<CandidatoFormacao>();
            if (formacoes.Any(f => f.Titulo == formacao.Titulo && f.Area == formacao.Area))
            {
                TempData.Put("formacoes-candidato", formacoes);
                return Json(new { mensagem = "A formação informada já foi adicionada" });
            }

            formacoes.Add(formacao);
            TempData.Put("formacoes-candidato", formacoes);
            return PartialView("_Formacao", formacoes);
        }

        [HttpPost]
        public IActionResult RemoverFormacao(string titulo, string area)
        {
            if (area == null)
                area = "";

            var formacoes = TempData.Get<List<CandidatoFormacao>>("formacoes-candidato") ?? new List<CandidatoFormacao>();
            var formacaoRemover = formacoes.FirstOrDefault(f => f.Titulo == titulo && f.Area == area);
            formacoes.Remove(formacaoRemover);
            TempData.Put("formacoes-candidato", formacoes);
            return PartialView("_Formacao", formacoes);
        }

        public IActionResult TelaNovoCurso() => PartialView("_NovoCurso");

        [HttpPost]
        public IActionResult IncluirCurso(CandidatoCurso curso)
        {
            var cursos = TempData.Get<List<CandidatoCurso>>("cursos-candidato") ?? new List<CandidatoCurso>();
            if (cursos.Any(c => c.Titulo == curso.Titulo && c.InstituicaoEnsino == curso.InstituicaoEnsino
                            && c.DuracaoHoras == curso.DuracaoHoras && c.AnoConclusao == curso.AnoConclusao))
            {
                TempData.Put("cursos-candidato", cursos);
                return Json(new { mensagem = "O curso informado já foi adicionado" });
            }

            cursos.Add(curso);
            TempData.Put("cursos-candidato", cursos);
            return PartialView("_Cursos", cursos);
        }

        [HttpPost]
        public IActionResult RemoverCurso(string titulo, double duracaoHoras, string instituicao)
        {
            var cursos = TempData.Get<List<CandidatoCurso>>("cursos-candidato") ?? new List<CandidatoCurso>();
            cursos.Remove(cursos.FirstOrDefault(c => c.Titulo == titulo && c.DuracaoHoras == duracaoHoras && c.InstituicaoEnsino == instituicao));
            TempData.Put("cursos-candidato", cursos);
            return PartialView("_Cursos", cursos);
        }

        public IActionResult TelaNovaExperiencia() => PartialView("_NovaExperiencia");

        [HttpPost]
        public IActionResult IncluirExperiencia(CandidatoExperiencia experiencia)
        {
            if (string.IsNullOrWhiteSpace(experiencia.Duracao))
            {
                if (experiencia.Inicio.HasValue && experiencia.Inicio != DateTime.MinValue)
                {
                    var dataFinal = experiencia.TrabalhoAtual ? DateTime.Now : experiencia.Termino == null ? DateTime.Now : !experiencia.Termino.HasValue ? DateTime.Now : experiencia.Termino;

                    var duracao = dataFinal.Value - experiencia.Inicio.Value;

                    int anos = (int)(duracao.Days / 365.25);
                    int meses = (int)((duracao.Days % 365.25) / 30);

                    var compAnos = "";
                    var compMeses = "";
                    if (anos != 1)
                        compAnos = "s";

                    if (meses != 1)
                        compMeses = "es";

                    experiencia.Duracao = $"{anos} ano{compAnos} e {meses} mes{compMeses}";
                }
            }

            var experiencias = TempData.Get<List<CandidatoExperiencia>>("experiencias-candidato") ?? new List<CandidatoExperiencia>();
            if (string.IsNullOrWhiteSpace(experiencia.Cargo) || string.IsNullOrWhiteSpace(experiencia.Empregador))
            {
                TempData.Put("experiencias-candidato", experiencias);
                return Json(new { mensagem = "Não foi informado o cargo e/ou empregador" });
            }

            experiencias.Add(experiencia);
            TempData.Put("experiencias-candidato", experiencias);
            return PartialView("_Experiencias", experiencias);
        }

        public IActionResult RemoverExperiencia(string empregador, string inicio, string cargo)
        {
            var experiencias = TempData.Get<List<CandidatoExperiencia>>("experiencias-candidato") ?? new List<CandidatoExperiencia>();
            DateTime dataInicio;
            if (!DateTime.TryParse(inicio, out dataInicio))
            {
                return BadRequest("A data de início fornecida não é válida.");
            }
            experiencias.Remove(experiencias.FirstOrDefault(e => e.Empregador == empregador && e.Inicio == dataInicio && e.Cargo == cargo));
            TempData.Put("experiencias-candidato", experiencias);
            return PartialView("_Experiencias", experiencias);
        }

        [HttpPost]
        public async Task<JsonResult> Incluir(CandidatoViewModel candidatoViewModel)
        {
            try
            {
                var candidato = _mapper.Map<Candidato>(candidatoViewModel);

                candidato.UsuarioInclusao = User.Identity.Name;
                candidato.DataInclusao = DateTime.Now;
                candidato.Contatos = TempData.Get<List<CandidatoContato>>("contatos-candidato") ?? new List<CandidatoContato>();
                candidato.Formacao = TempData.Get<List<CandidatoFormacao>>("formacoes-candidato") ?? new List<CandidatoFormacao>();
                candidato.Cursos = TempData.Get<List<CandidatoCurso>>("cursos-candidato") ?? new List<CandidatoCurso>();
                candidato.Experiencias = TempData.Get<List<CandidatoExperiencia>>("experiencias-candidato") ?? new List<CandidatoExperiencia>();
                var resultado = await _candidatosService.Incluir(candidato);

                if (!resultado.Valido)
                {
                    return Json(new
                    {
                        resultado = "falha",
                        mensagem = string.Join(".", resultado.Mensagens)
                    });
                };

                TempData["success"] = "Candidato Incluído com Sucesso!";
                return Json(new { resultado = "sucesso" });
            }
            catch (Exception e)
            {
                return Json(new { resultado = "falha", mensagem = string.Join(", ", e) });
            }
        }


        public async Task<IActionResult> Editar(int id)
        {
            TempData.Remove("contatos-candidato");
            TempData.Remove("formacoes-candidato");
            TempData.Remove("cursos-candidato");
            TempData.Remove("experiencias-candidato");
            var candidato = (await _candidatosRepository.BuscarCandidatos(new CandidatoParams()
            {
                Id = id,
                InfoExtras = true
            })).FirstOrDefault();

            TempData.Put("contatos-candidato", candidato.Contatos);
            TempData.Put("formacoes-candidato", candidato.Formacao);
            TempData.Put("cursos-candidato", candidato.Cursos);
            TempData.Put("experiencias-candidato", candidato.Experiencias);

            return View(_mapper.Map<CandidatoViewModel>(candidato));
        }

        [HttpPost]
        public async Task<JsonResult> Editar(CandidatoViewModel candidatoViewModel)
        {
            try
            {
                var candidato = _mapper.Map<Candidato>(candidatoViewModel);
                candidato.Contatos = TempData.Get<List<CandidatoContato>>("contatos-candidato") ?? new List<CandidatoContato>();
                candidato.Formacao = TempData.Get<List<CandidatoFormacao>>("formacoes-candidato") ?? new List<CandidatoFormacao>();
                candidato.Cursos = TempData.Get<List<CandidatoCurso>>("cursos-candidato") ?? new List<CandidatoCurso>();
                candidato.Experiencias = TempData.Get<List<CandidatoExperiencia>>("experiencias-candidato") ?? new List<CandidatoExperiencia>();

                var resultado = await _candidatosService.Editar(candidato);

                if (!resultado.Valido)
                {
                    TempData.Put("contatos-candidato", candidato.Contatos);
                    TempData.Put("formacoes-candidato", candidato.Formacao);
                    TempData.Put("cursos-candidato", candidato.Cursos);
                    TempData.Put("experiencias-candidato", candidato.Experiencias);

                    return Json(new
                    {
                        resultado = "falha",
                        mensagem = string.Join(".", resultado.Mensagens)
                    });
                };

                TempData["success"] = "Candidato Editado com Sucesso!";
                return Json(new { resultado = "sucesso" });
            }
            catch (Exception e)
            {
                return Json(new { resultado = "falha", mensagem = string.Join(", ", e) });
            }
        }


        [HttpPost]
        public async Task<JsonResult> AlterarStatus(int id)
        {
            try
            {
                var candidato = (await _candidatosRepository.BuscarCandidatos(new CandidatoParams(){Id = id, InfoExtras = true})).FirstOrDefault();

                if (candidato == null)
                    return Json(new {resultado = "falha",mensagem = "candidato não encontrado"});

                var resultado = await _candidatosService.AlterarStatus(candidato);

                if (!resultado.Valido)
                    return Json(new { resultado = "falha", mensagem = string.Join(".", resultado.Mensagens) });

                return Json(new { resultado = "sucesso" });
            }
            catch (Exception e)
            {
                return Json(new { resultado = "falha", mensagem = string.Join(", ", e) });
            }
        }

        public async Task<IActionResult> BuscarCurriculo(int id)
        {
            var candidato = (await _candidatosRepository.BuscarCandidatos(new CandidatoParams() { Id = id, InfoExtras = true })).FirstOrDefault();

            if (candidato == null)
                return Json(new { resultado = "falha", mensagem = "candidato não encontrado" });

            return PartialView("_Curriculo", candidato);
        }
    }
}
