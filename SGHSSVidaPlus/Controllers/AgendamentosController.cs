using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams; // Certifique-se que AgendamentoParams e ProfissionalSaudeParams estão aqui
using SGHSSVidaPlus.Domain.Interfaces.Repository;
using SGHSSVidaPlus.Domain.Interfaces.Service;
using SGHSSVidaPlus.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGHSSVidaPlus.MVC.Controllers
{
    [Authorize]
    public class AgendamentosController : Controller
    {
        private readonly IAgendamentoRepository _agendamentoRepository;
        private readonly IAgendamentoService _agendamentoService;
        private readonly IPacienteRepository _pacienteRepository;
        private readonly ITipoAtendimentoRepository _tipoAtendimentoRepository;
        private readonly IProfissionalSaudeRepository _profissionalSaudeRepository;
        private readonly IMapper _mapper;

        public AgendamentosController(
            IAgendamentoRepository agendamentoRepository,
            IAgendamentoService agendamentoService,
            IPacienteRepository pacienteRepository,
            ITipoAtendimentoRepository tipoAtendimentoRepository,
            IProfissionalSaudeRepository profissionalSaudeRepository,
            IMapper mapper)
        {
            _agendamentoRepository = agendamentoRepository;
            _agendamentoService = agendamentoService;
            _pacienteRepository = pacienteRepository;
            _tipoAtendimentoRepository = tipoAtendimentoRepository;
            _profissionalSaudeRepository = profissionalSaudeRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var agendamentos = await _agendamentoRepository.BuscarAgendamentos(new AgendamentoParams());
            // Se sua view Index espera uma ViewModel, descomente a linha abaixo e mapeie:
            // var viewModelList = _mapper.Map<List<AgendamentoViewModel>>(agendamentos);
            // return View(viewModelList);
            return View(agendamentos);
        }

        public IActionResult Incluir()
        {
            return View(new AgendamentoViewModel());
        }

        [HttpPost]
        public async Task<JsonResult> Incluir([FromBody] AgendamentoViewModel agendamentoViewModel) // Adicione [FromBody] se você vai enviar JSON via AJAX
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", erros) });
                }

                var agendamento = _mapper.Map<Agendamento>(agendamentoViewModel);

                agendamento.UsuarioInclusao = User.Identity.Name ?? "Sistema";
                agendamento.DataInclusao = DateTime.Now;
                agendamento.Encerrado = false;
                agendamento.UsuarioEncerramento = null;
                agendamento.DataEncerramento = null;

                // Os IDs já virão preenchidos da ViewModel
                // agendamento.ProfissionalResponsavelId = agendamentoViewModel.ProfissionalResponsavelId; 
                // agendamento.PacienteId = agendamentoViewModel.PacienteId;

                var resultado = await _agendamentoService.Incluir(agendamento);

                if (!resultado.Valido)
                {
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });
                }

                TempData["success"] = "Agendamento incluído com sucesso!";
                return Json(new { resultado = "sucesso", redirectUrl = Url.Action("Index", "Agendamentos") }); // Adicionado redirectUrl
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao incluir agendamento: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return Json(new { resultado = "falha", mensagem = "Ocorreu um erro inesperado ao incluir o agendamento: " + ex.Message });
            }
        }

        // Action para obter a lista de profissionais de saúde para o modal de seleção
        // Já está boa, apenas confirme que ProfissionalSaudeParams e o .Where(p => p.Ativo) funcionam conforme seu repositório
        public async Task<IActionResult> ObterProfissionaisParaSelecao()
        {
            try
            {
                var parametros = new ProfissionalSaudeParams { Ativo = true };
                var profissionais = await _profissionalSaudeRepository.BuscarProfissional(parametros);

                var viewModelList = _mapper.Map<List<ProfissionalSaudeViewModel>>(profissionais);

                return PartialView("_TabelaProfissionaisParaSelecao", viewModelList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar lista de profissionais: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Response.StatusCode = 500;
                return Content("Erro interno do servidor ao carregar profissionais.");
            }
        }

        // Action para obter a lista de pacientes para o modal de seleção
        // Já está boa, apenas confirme que _pacienteRepository.ObterTodos() retorna apenas ativos,
        // ou ajuste para usar um parâmetro como no profissional
        public async Task<IActionResult> ObterPacientesParaSelecao()
        {
            try
            {
                // Sugestão: Crie um PacienteParams para filtrar ativos, se ObterTodos não o fizer
                // var parametros = new PacienteParams { Ativo = true };
                // var pacientes = await _pacienteRepository.BuscarPacientes(parametros);
                var pacientes = await _pacienteRepository.ObterTodos(); // Se ObterTodos já traz só ativos ou você não precisa filtrar

                var viewModelList = _mapper.Map<List<PacienteViewModel>>(pacientes);

                return PartialView("_TabelaPacientesParaSelecao", viewModelList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar lista de pacientes: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Response.StatusCode = 500;
                return Content("Erro interno do servidor ao carregar pacientes.");
            }
        }

        // ... (Seu código de Editar e outras ações)
        public async Task<IActionResult> Editar(int id)
        {
            var agendamento = (await _agendamentoRepository.BuscarAgendamentos(new AgendamentoParams { Id = id, IncluirProfissional = true, IncluirPaciente = true })).FirstOrDefault();
            if (agendamento == null)
            {
                TempData["error"] = "Agendamento não encontrado.";
                return RedirectToAction("Index");
            }
            var viewModel = _mapper.Map<AgendamentoViewModel>(agendamento);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> Editar([FromBody] AgendamentoViewModel agendamentoViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", erros) });
                }

                var agendamento = _mapper.Map<Agendamento>(agendamentoViewModel);

                var resultado = await _agendamentoService.Editar(agendamento);

                if (!resultado.Valido)
                {
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });
                }
                TempData["success"] = "Agendamento editado com sucesso!";
                return Json(new { resultado = "sucesso", redirectUrl = Url.Action("Index", "Agendamentos") });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao editar agendamento: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return Json(new { resultado = "falha", mensagem = "Ocorreu um erro inesperado ao editar o agendamento: " + ex.Message });
            }
        }
    }
}