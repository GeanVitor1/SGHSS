using Microsoft.AspNetCore.Mvc;
using Selecao.Domain.Interfaces.Repository;
using Selecao.Domain.Entities;
using Selecao.MVC.Models;
using AutoMapper;
using Selecao.Domain.Interfaces.Service;
using Selecao.Domain.Extensions_Params;
using Microsoft.AspNetCore.Authorization;

namespace Selecao.MVC.Controllers
{
    [Authorize]
    public class SelecaoController : Controller
    {
        private readonly ISelecaoRepository _selecaoRepository;
        private readonly ISelecaoService _selecaoService;
        private readonly ICandidatosRepository _candidatosRepository;
        private readonly IEtapaSelecaoRepository _etapaSelecaoRepository;
        private readonly IMapper _mapper;

        public SelecaoController(ISelecaoRepository selecaoRepository, ISelecaoService selecaoService, ICandidatosRepository candidatosRepository, IEtapaSelecaoRepository etapaSelecaoRepository, IMapper mapper)
        {
            _selecaoRepository = selecaoRepository;
            _selecaoService = selecaoService;
            _candidatosRepository = candidatosRepository;
            _etapaSelecaoRepository = etapaSelecaoRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index() => View(await _selecaoRepository.BuscarSelecoes(new SelecaoParams()));

        public async Task<IActionResult> Incluir() => View();

        [HttpPost]
        public async Task<JsonResult> Incluir(SelecaoCandidatoViewModel selecaoViewModel)
        {
            try
            {
                var selecao = _mapper.Map<SelecaoCandidato>(selecaoViewModel);
                selecao.UsuarioInclusao = User.Identity.Name;
                selecao.DataInclusao = DateTime.Now;

                var resultado = await _selecaoService.Incluir(selecao);

                if (!resultado.Valido)
                    return Json(new { resultado = "falha", mensagem = string.Join(".", resultado.Mensagens) });


                TempData["success"] = "Seleção Incluída com Sucesso!";
                return Json(new { resultado = "sucesso" });
            }
            catch (Exception e)
            {
                return Json(new { resultado = "falha", mensagem = string.Join(", ", e) });
            }
        }

        public async Task<IActionResult> IncluirCandidatos(int id)
        {
            var selecaoBd = (await _selecaoRepository.BuscarSelecoes(new SelecaoParams { Id = id, InfoExtras = true })).FirstOrDefault();
            var candidatosAtuais = selecaoBd.Candidatos;

            selecaoBd.Candidatos = null;
            selecaoBd.Etapas = null;
            var selecao = _mapper.Map<SelecaoCandidatoViewModel>(selecaoBd);

            var candidatos = await _candidatosRepository.BuscarCandidatos(new CandidatoParams() { Banido = false, Selecionado = false, TotalRegistros = 0, InfoExtras = true });

            selecao.Candidatos = (from c in candidatos
                                  select new CandidatoSelectViewModel
                                  {
                                      Id = c.Id,
                                      Nome = c.Nome,
                                      DataNascimento = c.DataNascimento,
                                      Logradouro = c.Logradouro,
                                      Bairro = c.Bairro,
                                      EstadoCivil = c.EstadoCivil,
                                      Selecionado = c.Selecionado,
                                      Banido = c.Banido,
                                      UsuarioInclusao = c.UsuarioInclusao,
                                      DataInclusao = c.DataInclusao,
                                      Contatos = c.Contatos,
                                      Experiencias = c.Experiencias,
                                      Formacao = c.Formacao,
                                      Cursos = c.Cursos,
                                      IsSelected = false,
                                  }).ToList();


            foreach(var candidato in selecao.Candidatos)
            {
                if (candidatosAtuais.Any(c => c.CandidatoId == candidato.Id))
                    candidato.IsSelected = true;

            }

            return View(selecao);
        }

        public async Task<IActionResult> FiltrarCandidatos(SelecaoCandidatoViewModel selecao)
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<JsonResult> IncluirCandidatos(SelecaoCandidatoViewModel selecaoViewModel)
        {
            try
            {
                var candidatoSelecao = (from c in selecaoViewModel.Candidatos.Where(c => c.IsSelected)
                                        select new CandidatoSelecaoCandidato
                                        {
                                            CandidatoId = c.Id,
                                            SelecaoId = selecaoViewModel.Id
                                        }).ToList();


                var resultado = await _selecaoService.IncluirCandidatos(candidatoSelecao, selecaoViewModel.Id);

                if (!resultado.Valido)
                    return Json(new { resultado = "falha", mensagem = string.Join(".", resultado.Mensagens) });


                TempData["success"] = "Seleção Incluída com Sucesso!";
                return Json(new { resultado = "sucesso" });
            }
            catch (Exception e)
            {
                return Json(new { resultado = "falha", mensagem = string.Join(", ", e) });
            }
        }

        public async Task<IActionResult> IncluirEtapas(int id)
        {
            var selecaoBd = (await _selecaoRepository.BuscarSelecoes(new SelecaoParams { Id = id, InfoExtras = true })).FirstOrDefault();
            var etapasAtuais = selecaoBd.Etapas;

            selecaoBd.Candidatos = null;
            selecaoBd.Etapas = null;
            var selecao = _mapper.Map<SelecaoCandidatoViewModel>(selecaoBd);

            var etapas = await _etapaSelecaoRepository.BuscarEtapas(new EtapaSelecaoParams() { Status = true, TotalRegistros = 0 });

            selecao.Etapas = (from e in etapas
                              select new EtapaSelectViewModel
                              {
                                  Id = e.Id,
                                  Nome = e.Nome,
                                  IsSelected = false
                              }).ToList();

            foreach (var etapa in selecao.Etapas)
            {
                if (etapasAtuais.Any(c => c.Etapa.Id == etapa.Id))
                    etapa.IsSelected = true;
            }

            return View(selecao);
        }

        [HttpPost]
        public async Task<JsonResult> IncluirEtapas(SelecaoCandidatoViewModel selecaoViewModel)
        {
            try
            {
                var candidatoSelecao = (from c in selecaoViewModel.Candidatos.Where(c => c.IsSelected)
                                        select new CandidatoSelecaoCandidato
                                        {
                                            CandidatoId = c.Id,
                                            SelecaoId = selecaoViewModel.Id
                                        }).ToList();


                var resultado = await _selecaoService.IncluirCandidatos(candidatoSelecao, selecaoViewModel.Id);

                if (!resultado.Valido)
                    return Json(new { resultado = "falha", mensagem = string.Join(".", resultado.Mensagens) });


                TempData["success"] = "Seleção Incluída com Sucesso!";
                return Json(new { resultado = "sucesso" });
            }
            catch (Exception e)
            {
                return Json(new { resultado = "falha", mensagem = string.Join(", ", e) });
            }
        }
    }
}
