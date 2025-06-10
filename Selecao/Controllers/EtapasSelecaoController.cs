using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Selecao.Domain.Entities;
using Selecao.Domain.Extensions_Params;
using Selecao.Domain.Interfaces.Repository;
using Selecao.Domain.Interfaces.Service;
using Selecao.MVC.Models;

namespace Selecao.MVC.Controllers
{
    [Authorize]
    public class EtapasSelecaoController : Controller
    {

        private readonly IEtapaSelecaoRepository _etapaSelecaoRepository;
        private readonly IEtapasSelecaoService _etapasSelecaoService;
        private readonly IMapper _mapper;

        public EtapasSelecaoController(IEtapaSelecaoRepository etapaSelecaoRepository, IEtapasSelecaoService etapasSelecaoService, IMapper mapper)
        {
            _etapaSelecaoRepository = etapaSelecaoRepository;
            _etapasSelecaoService = etapasSelecaoService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index() => View(await _etapaSelecaoRepository.BuscarEtapas(new EtapaSelecaoParams() { StatusString = "1"}));

        public async Task<IActionResult> BuscarEtapas([FromQuery]EtapaSelecaoParams parametros) => PartialView("_Etapas", await _etapaSelecaoRepository.BuscarEtapas(parametros));

        public IActionResult Incluir() => PartialView("_Incluir");

        [HttpPost]
        public async Task<JsonResult> Incluir(EtapaSelecaoViewModel etapaSelecaoViewModel)
        {
            var etapa= _mapper.Map<EtapaSelecao>(etapaSelecaoViewModel);
            etapa.UsuarioInclusao = User.Identity.Name;
            etapa.DataInclusao = DateTime.Now;
            etapa.Status = true;

            var resultado = await _etapasSelecaoService.Incluir(etapa);

            if (!resultado.Valido)
                return Json(new { resultado = "falha", mensagem = string.Join(".", resultado.Mensagens)});


            return Json(new { resultado = "sucesso" });
        }

        public async Task<IActionResult> Editar(int id) //=> PartialView("_Incluir", _mapper.Map<EtapaSelecaoViewModel>((await _etapaSelecaoRepository.BuscarEtapas(new EtapaSelecaoParams() { Id = id })).FirstOrDefault()));
        {
            var etapa = (await _etapaSelecaoRepository.BuscarEtapas(new EtapaSelecaoParams() { Id = id })).FirstOrDefault();
            return PartialView("_Incluir", _mapper.Map<EtapaSelecaoViewModel>(etapa));
        }

        [HttpPut]
        public async Task<JsonResult> Editar(EtapaSelecaoViewModel etapaSelecaoViewModel)
        {
            var etapa = _mapper.Map<EtapaSelecao>(etapaSelecaoViewModel);

            var resultado = await _etapasSelecaoService.Editar(etapa);

            if (!resultado.Valido)
                return Json(new { resultado = "falha", mensagem = string.Join(".", resultado.Mensagens) });


            return Json(new { resultado = "sucesso" });
        }


        [HttpPut]
        public async Task<JsonResult> AlterarStatus(int id)
        {
            var etapa = await _etapaSelecaoRepository.ObterPorId(id);

            if(etapa == null)
                return Json(new{resultado = "falha",mensagem = "Id inválido" });

            
            var resultado = await _etapasSelecaoService.AlterarStatus(etapa);
            if (!resultado.Valido)
                return Json(new { resultado = "falha", mensagem = string.Join(".", resultado.Mensagens) });


            return Json(new { resultado = "sucesso" });
        }
    }
}
