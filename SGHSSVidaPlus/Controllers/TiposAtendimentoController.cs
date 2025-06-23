using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGHSSVidaPlus.Domain.Entities; // Namespace atualizado
using SGHSSVidaPlus.Domain.ExtensionsParams; // Namespace atualizado
using SGHSSVidaPlus.Domain.Interfaces.Repository; // Namespace atualizado
using SGHSSVidaPlus.Domain.Interfaces.Service; // Namespace atualizado
using SGHSSVidaPlus.MVC.Models; // Namespace atualizado
using System; // Para DateTime
using System.Linq; // Para LINQ
using System.Threading.Tasks; // Para Task

namespace SGHSSVidaPlus.MVC.Controllers // Namespace atualizado
{
    [Authorize]
    public class TiposAtendimentoController : Controller // Nome da classe atualizado
    {
        private readonly ITipoAtendimentoRepository _tipoAtendimentoRepository; // Nome da injeção atualizado
        private readonly ITipoAtendimentoService _tipoAtendimentoService; // Nome da injeção e interface atualizados
        private readonly IMapper _mapper;

        public TiposAtendimentoController(ITipoAtendimentoRepository tipoAtendimentoRepository, ITipoAtendimentoService tipoAtendimentoService, IMapper mapper)
        {
            _tipoAtendimentoRepository = tipoAtendimentoRepository;
            _tipoAtendimentoService = tipoAtendimentoService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index() => View(await _tipoAtendimentoRepository.BuscarTiposAtendimento(new TipoAtendimentoParams() { StatusAtivoString = "1" })); // Parâmetro atualizado

        public async Task<IActionResult> BuscarEtapas([FromQuery] TipoAtendimentoParams parametros) => PartialView("_Etapas", await _tipoAtendimentoRepository.BuscarTiposAtendimento(parametros)); // Nome do método e tipo de retorno atualizados

        public IActionResult Incluir() => PartialView("_Incluir");

        [HttpPost]
        public async Task<JsonResult> Incluir(TipoAtendimentoViewModel tipoAtendimentoViewModel) // Nome do ViewModel atualizado
        {
            var tipoAtendimento = _mapper.Map<TipoAtendimento>(tipoAtendimentoViewModel); // Nome da entidade atualizado
            tipoAtendimento.UsuarioInclusao = User.Identity.Name;
            tipoAtendimento.DataInclusao = DateTime.Now;
            tipoAtendimento.Ativo = true; // Propriedade Status agora é Ativo

            var resultado = await _tipoAtendimentoService.Incluir(tipoAtendimento); // Nome do método e entidade atualizados

            if (!resultado.Valido)
                return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });

            return Json(new { resultado = "sucesso" });
        }

        public async Task<IActionResult> Editar(int id)
        {
            var tipoAtendimento = (await _tipoAtendimentoRepository.BuscarTiposAtendimento(new TipoAtendimentoParams() { Id = id })).FirstOrDefault(); // Entidade e parâmetro atualizados
            return PartialView("_Incluir", _mapper.Map<TipoAtendimentoViewModel>(tipoAtendimento)); // ViewModel atualizado
        }

        [HttpPut]
        public async Task<JsonResult> Editar(TipoAtendimentoViewModel tipoAtendimentoViewModel) // ViewModel atualizado
        {
            var tipoAtendimento = _mapper.Map<TipoAtendimento>(tipoAtendimentoViewModel); // Entidade atualizada

            var resultado = await _tipoAtendimentoService.Editar(tipoAtendimento); // Entidade atualizada

            if (!resultado.Valido)
                return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });

            return Json(new { resultado = "sucesso" });
        }


        [HttpPut]
        public async Task<JsonResult> AlterarStatus(int id)
        {
            var tipoAtendimento = await _tipoAtendimentoRepository.ObterPorId(id); // Entidade atualizada

            if (tipoAtendimento == null)
                return Json(new { resultado = "falha", mensagem = "Id inválido" });

            var resultado = await _tipoAtendimentoService.AlterarStatus(tipoAtendimento); // Entidade atualizada
            if (!resultado.Valido)
                return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });

            return Json(new { resultado = "sucesso" });
        }
    }
}