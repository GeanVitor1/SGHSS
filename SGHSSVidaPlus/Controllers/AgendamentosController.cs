using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams;
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
        private readonly IProfissionalSaudeRepository _profissionalSaudeRepository;
        private readonly IMapper _mapper;

        public AgendamentosController(
            IAgendamentoRepository agendamentoRepository,
            IAgendamentoService agendamentoService,
            IPacienteRepository pacienteRepository,
            IProfissionalSaudeRepository profissionalSaudeRepository,
            IMapper mapper)
        {
            _agendamentoRepository = agendamentoRepository;
            _agendamentoService = agendamentoService;
            _pacienteRepository = pacienteRepository;
            _profissionalSaudeRepository = profissionalSaudeRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var parametros = new AgendamentoParams
            {
                IncluirProfissional = true,
                IncluirPaciente = true // Certifique-se de incluir o paciente aqui para o Index também
            };

            var agendamentos = await _agendamentoRepository.BuscarAgendamentos(parametros);
            return View(agendamentos);
        }

        public IActionResult Incluir()
        {
            return View(new AgendamentoViewModel());
        }

        [HttpPost]
        public async Task<JsonResult> Incluir([FromBody] AgendamentoViewModel agendamentoViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", erros) });
                }

                if (agendamentoViewModel.PacienteId <= 0)
                {
                    return Json(new { resultado = "falha", mensagem = "É necessário informar o paciente para o agendamento." });
                }

                var agendamento = _mapper.Map<Agendamento>(agendamentoViewModel);
                agendamento.PacienteId = agendamentoViewModel.PacienteId; // Se for um paciente principal

                agendamento.UsuarioInclusao = User.Identity.Name ?? "Sistema";
                agendamento.DataInclusao = DateTime.Now;
                agendamento.Encerrado = false;

                var resultado = await _agendamentoService.Incluir(agendamento);

                if (!resultado.Valido)
                {
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });
                }

                return Json(new { resultado = "sucesso", redirectUrl = Url.Action("Index", "Agendamentos") });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao incluir agendamento: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return Json(new { resultado = "falha", mensagem = "Ocorreu um erro inesperado ao incluir o agendamento." });
            }
        }

        public async Task<IActionResult> Editar(int id)
        {
            var agendamento = (await _agendamentoRepository.BuscarAgendamentos(new AgendamentoParams
            {
                Id = id,
                IncluirProfissional = true,
                IncluirPaciente = true // Incluir o paciente para carregar os dados completos
            })).FirstOrDefault();

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

                if (agendamentoViewModel.PacienteId <= 0)
                {
                    return Json(new { resultado = "falha", mensagem = "É necessário informar o paciente para o agendamento." });
                }

                var agendamento = _mapper.Map<Agendamento>(agendamentoViewModel);
                agendamento.PacienteId = agendamentoViewModel.PacienteId; // Se for um paciente principal

                var resultado = await _agendamentoService.Editar(agendamento);

                if (!resultado.Valido)
                {
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });
                }

                return Json(new { resultado = "sucesso", redirectUrl = Url.Action("Index", "Agendamentos") });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao editar agendamento: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return Json(new { resultado = "falha", mensagem = "Ocorreu um erro inesperado ao editar o agendamento." });
            }
        }

        [HttpPost]
        public async Task<JsonResult> EncerrarAgendamento([FromBody] int agendamentoId) // Ou um ViewModel se precisar de mais dados
        {
            try
            {
                // Aqui você pode pegar o nome do usuário logado se precisar de auditoria
                var usuarioEncerramento = User.Identity.Name ?? "Sistema";

                var resultado = await _agendamentoService.EncerrarAgendamento(agendamentoId, usuarioEncerramento);

                if (!resultado.Valido)
                {
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });
                }

                return Json(new { resultado = "sucesso", mensagem = resultado.Mensagens.FirstOrDefault() ?? "Agendamento encerrado com sucesso!", redirectUrl = Url.Action("Index", "Agendamentos") });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao encerrar agendamento: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return Json(new { resultado = "falha", mensagem = "Ocorreu um erro inesperado ao encerrar o agendamento." });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ReabrirAgendamento([FromBody] int agendamentoId)
        {
            try
            {
                var usuarioReabertura = User.Identity.Name ?? "Sistema"; // Ou algum usuário de auditoria

                // Crie um método correspondente no seu AgendamentoService:
                // public async Task<OperationResult> ReabrirAgendamento(int agendamentoId, string usuarioReabertura)
                var resultado = await _agendamentoService.ReabrirAgendamento(agendamentoId, usuarioReabertura); // <--- Este método precisa existir no serviço

                if (!resultado.Valido)
                {
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });
                }

                return Json(new { resultado = "sucesso", mensagem = resultado.Mensagens.FirstOrDefault() ?? "Agendamento reaberto com sucesso!", redirectUrl = Url.Action("Index", "Agendamentos") });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao reabrir agendamento: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return Json(new { resultado = "falha", mensagem = "Ocorreu um erro inesperado ao reabrir o agendamento." });
            }
        }

        // NOVO MÉTODO VISUALIZAR
        public async Task<IActionResult> Visualizar(int id)
        {
            // Busca o agendamento por ID e inclui as entidades relacionadas (Profissional e Paciente)
            // É crucial incluir estas entidades para que os nomes sejam exibidos na view.
            var agendamento = (await _agendamentoRepository.BuscarAgendamentos(new AgendamentoParams
            {
                Id = id,
                IncluirProfissional = true, // Para ter acesso ao nome do profissional
                IncluirPaciente = true      // Para ter acesso ao nome do paciente
                // Se você tiver tipos de atendimento ou outros relacionados para visualizar, inclua aqui:
                // IncluirPacientesTiposAtendimento = true
            })).FirstOrDefault();

            if (agendamento == null)
            {
                TempData["error"] = "Agendamento não encontrado.";
                return RedirectToAction("Index");
            }

            // Mapeia a entidade Agendamento para a AgendamentoViewModel
            var viewModel = _mapper.Map<AgendamentoViewModel>(agendamento);

            // Retorna a view "Visualizar" com a ViewModel populada
            return View("Visualizar", viewModel);
        }

        // --- MÉTODOS PARA OS MODAIS ---
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

        public async Task<IActionResult> ObterPacientesParaSelecao()
        {
            try
            {
                var pacientes = await _pacienteRepository.ObterTodos();
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
    }
}