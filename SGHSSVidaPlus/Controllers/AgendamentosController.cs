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
using System.Security.Claims; // Necessário para acessar as claims do usuário

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

        // Action para administradores verem todos os agendamentos
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            var parametros = new AgendamentoParams
            {
                IncluirProfissional = true,
                IncluirPaciente = true
            };

            var agendamentos = await _agendamentoRepository.BuscarAgendamentos(parametros);
            return View(agendamentos);
        }

        // NOVO: Action para pacientes verem seus próprios agendamentos
        [Authorize(Policy = "RequirePacienteRoleOrClaim")] // Ou uma policy mais específica
        public async Task<IActionResult> MeusAgendamentos()
        {
            // Obtém o ApplicationUserId do usuário logado
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(applicationUserId))
            {
                TempData["error"] = "Não foi possível identificar seu usuário.";
                return RedirectToAction("Index", "Home");
            }

            // Busca o paciente associado a este ApplicationUserId
            var paciente = (await _pacienteRepository.BuscarPacientes(new PacienteParams { ApplicationUserId = applicationUserId })).FirstOrDefault();

            if (paciente == null)
            {
                TempData["error"] = "Seu perfil de paciente não foi encontrado. Por favor, entre em contato com o suporte.";
                return RedirectToAction("Index", "Home");
            }

            var parametros = new AgendamentoParams
            {
                PacienteId = paciente.Id, // Filtra pelo ID do paciente logado
                IncluirProfissional = true,
                IncluirPaciente = true
            };

            var agendamentos = await _agendamentoRepository.BuscarAgendamentos(parametros);
            return View("Index", agendamentos); // Reutiliza a View Index para exibir a lista
        }

        [Authorize(Roles = "admin")] // Somente admin pode incluir agendamentos manualmente
        public IActionResult Incluir()
        {
            return View(new AgendamentoViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "admin")] // Somente admin pode incluir agendamentos manualmente
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
                agendamento.PacienteId = agendamentoViewModel.PacienteId;

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

        [Authorize(Roles = "admin")] // Somente admin pode editar
        public async Task<IActionResult> Editar(int id)
        {
            var agendamento = (await _agendamentoRepository.BuscarAgendamentos(new AgendamentoParams
            {
                Id = id,
                IncluirProfissional = true,
                IncluirPaciente = true
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
        [Authorize(Roles = "admin")] // Somente admin pode editar
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
                agendamento.PacienteId = agendamentoViewModel.PacienteId;

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
        [Authorize] // Ambos, admin e paciente, podem encerrar/cancelar
        public async Task<JsonResult> EncerrarAgendamento([FromBody] int agendamentoId)
        {
            try
            {
                var usuarioEncerramento = User.Identity.Name ?? "Sistema";

                // Se o usuário não for admin, verificar se ele é o paciente dono do agendamento
                if (!User.IsInRole("admin"))
                {
                    var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var paciente = (await _pacienteRepository.BuscarPacientes(new PacienteParams { ApplicationUserId = applicationUserId })).FirstOrDefault();

                    if (paciente == null)
                    {
                        return Json(new { resultado = "falha", mensagem = "Paciente não encontrado ou não autorizado a cancelar este agendamento." });
                    }

                    var agendamentoToCheck = (await _agendamentoRepository.BuscarAgendamentos(new AgendamentoParams { Id = agendamentoId })).FirstOrDefault();

                    if (agendamentoToCheck == null || agendamentoToCheck.PacienteId != paciente.Id)
                    {
                        return Json(new { resultado = "falha", mensagem = "Você não tem permissão para cancelar este agendamento." });
                    }
                }


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
        [Authorize(Roles = "admin")] // Somente admin pode reabrir
        public async Task<JsonResult> ReabrirAgendamento([FromBody] int agendamentoId)
        {
            try
            {
                var usuarioReabertura = User.Identity.Name ?? "Sistema";

                var resultado = await _agendamentoService.ReabrirAgendamento(agendamentoId, usuarioReabertura);

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

        // Visualizar agendamento (acessível por admin e paciente, mas paciente só vê o dele)
        [Authorize]
        public async Task<IActionResult> Visualizar(int id)
        {
            // Se não for admin, verifica a permissão
            if (!User.IsInRole("admin"))
            {
                var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var paciente = (await _pacienteRepository.BuscarPacientes(new PacienteParams { ApplicationUserId = applicationUserId })).FirstOrDefault();

                if (paciente == null)
                {
                    TempData["error"] = "Seu perfil de paciente não foi encontrado.";
                    return RedirectToAction("MeusAgendamentos");
                }

                var agendamentoToCheck = (await _agendamentoRepository.BuscarAgendamentos(new AgendamentoParams { Id = id })).FirstOrDefault();

                if (agendamentoToCheck == null || agendamentoToCheck.PacienteId != paciente.Id)
                {
                    TempData["error"] = "Você não tem permissão para visualizar este agendamento.";
                    return RedirectToAction("MeusAgendamentos");
                }
            }

            var agendamento = (await _agendamentoRepository.BuscarAgendamentos(new AgendamentoParams
            {
                Id = id,
                IncluirProfissional = true,
                IncluirPaciente = true
            })).FirstOrDefault();

            if (agendamento == null)
            {
                TempData["error"] = "Agendamento não encontrado.";
                return RedirectToAction("Index"); // Ou MeusAgendamentos se for paciente
            }

            var viewModel = _mapper.Map<AgendamentoViewModel>(agendamento);
            return View("Visualizar", viewModel);
        }

        // --- MÉTODOS PARA OS MODAIS (Acessíveis apenas para admin, pois a inclusão manual é admin-only) ---
        [Authorize(Roles = "admin")]
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

        [Authorize(Roles = "admin")]
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