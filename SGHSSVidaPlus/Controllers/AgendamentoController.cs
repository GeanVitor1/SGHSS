using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGHSSVidaPlus.Domain.Entities; // Namespace atualizado
using SGHSSVidaPlus.Domain.ExtensionsParams; // Namespace atualizado
using SGHSSVidaPlus.Domain.Interfaces.Repository; // Namespace atualizado
using SGHSSVidaPlus.Domain.Interfaces.Service; // Namespace atualizado
using SGHSSVidaPlus.MVC.Models; // Namespace atualizado
using System; // Para DateTime
using System.Collections.Generic; // Para List
using System.Linq; // Para LINQ
using System.Threading.Tasks; // Para Task

namespace SGHSSVidaPlus.MVC.Controllers // Namespace atualizado
{
    [Authorize]
    public class AgendamentosController : Controller // Nome da classe atualizado
    {
        private readonly IAgendamentoRepository _agendamentoRepository; // Nome da injeção e interface atualizados
        private readonly IAgendamentoService _agendamentoService; // Nome da injeção e interface atualizados
        private readonly IPacienteRepository _pacienteRepository; // Nome da injeção e interface atualizados (antigo _candidatosRepository)
        private readonly ITipoAtendimentoRepository _tipoAtendimentoRepository; // Nome da injeção e interface atualizados (antigo _etapaSelecaoRepository)
        private readonly IMapper _mapper;

        public AgendamentosController(IAgendamentoRepository agendamentoRepository, IAgendamentoService agendamentoService, IPacienteRepository pacienteRepository, ITipoAtendimentoRepository tipoAtendimentoRepository, IMapper mapper)
        {
            _agendamentoRepository = agendamentoRepository;
            _agendamentoService = agendamentoService;
            _pacienteRepository = pacienteRepository;
            _tipoAtendimentoRepository = tipoAtendimentoRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index() => View(await _agendamentoRepository.BuscarAgendamentos(new AgendamentoParams())); // Parâmetro atualizado

        public async Task<IActionResult> Incluir() => View();

        [HttpPost]
        public async Task<JsonResult> Incluir(AgendamentoViewModel agendamentoViewModel) // ViewModel atualizado
        {
            try
            {
                var agendamento = _mapper.Map<Agendamento>(agendamentoViewModel); // Entidade atualizada
                agendamento.UsuarioInclusao = User.Identity.Name;
                agendamento.DataInclusao = DateTime.Now;

                // Mapeia listas de ViewModels para Entities para inclusão
                agendamento.PacientesAgendados = _mapper.Map<List<AgendamentoPaciente>>(agendamentoViewModel.PacientesAgendados);
                agendamento.TiposAtendimento = _mapper.Map<List<AgendamentoTipoAtendimento>>(agendamentoViewModel.TiposAtendimento);


                var resultado = await _agendamentoService.Incluir(agendamento); // Serviço e entidade atualizados

                if (!resultado.Valido)
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });

                TempData["success"] = "Agendamento Incluído com Sucesso!";
                return Json(new { resultado = "sucesso" });
            }
            catch (Exception e)
            {
                return Json(new { resultado = "falha", mensagem = string.Join(" ", e.Message) });
            }
        }

        // Action para selecionar pacientes para um agendamento
        public async Task<IActionResult> IncluirPacientes(int id)
        {
            var agendamentoBd = (await _agendamentoRepository.BuscarAgendamentos(new AgendamentoParams { Id = id, IncluirPacientesTiposAtendimento = true })).FirstOrDefault(); // Parâmetro atualizado
            var pacientesAtuaisDoAgendamento = agendamentoBd?.PacientesAgendados ?? new List<AgendamentoPaciente>(); // Pega os pacientes já associados

            // Remove referências para evitar ciclos de serialização/mapeamento
            if (agendamentoBd != null)
            {
                agendamentoBd.PacientesAgendados = null;
                agendamentoBd.TiposAtendimento = null;
                if (agendamentoBd.ProfissionalResponsavel != null)
                {
                    agendamentoBd.ProfissionalResponsavel.AgendamentosRealizados = null; // Evita ciclo
                }
            }

            var agendamentoViewModel = _mapper.Map<AgendamentoViewModel>(agendamentoBd); // Mapeia o agendamento

            // Busca todos os pacientes que podem ser selecionados
            var todosPacientes = await _pacienteRepository.ObterTodos(); // Busca todos os pacientes

            // Mapeia todos os pacientes para o ViewModel de seleção e marca os já selecionados
            agendamentoViewModel.PacientesAgendados = (from p in todosPacientes
                                                       select new AgendamentoPacienteViewModel
                                                       {
                                                           PacienteId = p.Id,
                                                           Paciente = _mapper.Map<PacienteViewModel>(p), // Mapeia o paciente completo
                                                           IsSelected = pacientesAtuaisDoAgendamento.Any(ap => ap.PacienteId == p.Id) // Verifica se já está selecionado
                                                       }).ToList();

            return View(agendamentoViewModel);
        }

        [HttpPost]
        public async Task<JsonResult> IncluirPacientes(AgendamentoViewModel agendamentoViewModel) // ViewModel atualizado
        {
            try
            {
                // Filtra apenas os pacientes selecionados no formulário
                var pacientesSelecionados = agendamentoViewModel.PacientesAgendados
                                                                .Where(apvm => apvm.IsSelected)
                                                                .Select(apvm => new AgendamentoPaciente { PacienteId = apvm.PacienteId, AgendamentoId = agendamentoViewModel.Id, Compareceu = apvm.Compareceu, AtendimentoFinalizado = apvm.AtendimentoFinalizado }) // Cria a entidade de ligação
                                                                .ToList();

                var resultado = await _agendamentoService.IncluirPacientesNoAgendamento(pacientesSelecionados, agendamentoViewModel.Id); // Serviço e entidade atualizados

                if (!resultado.Valido)
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });

                TempData["success"] = "Pacientes adicionados ao Agendamento com Sucesso!";
                return Json(new { resultado = "sucesso" });
            }
            catch (Exception e)
            {
                return Json(new { resultado = "falha", mensagem = string.Join(" ", e.Message) });
            }
        }

        // Action para selecionar tipos de atendimento para um agendamento
        public async Task<IActionResult> IncluirTiposAtendimento(int id) // Nome do método atualizado
        {
            var agendamentoBd = (await _agendamentoRepository.BuscarAgendamentos(new AgendamentoParams { Id = id, IncluirPacientesTiposAtendimento = true })).FirstOrDefault(); // Parâmetro atualizado
            var tiposAtendimentoAtuaisDoAgendamento = agendamentoBd?.TiposAtendimento ?? new List<AgendamentoTipoAtendimento>(); // Pega os tipos já associados

            if (agendamentoBd != null)
            {
                agendamentoBd.PacientesAgendados = null; // Evita ciclo
                agendamentoBd.TiposAtendimento = null; // Evita ciclo
                if (agendamentoBd.ProfissionalResponsavel != null)
                {
                    agendamentoBd.ProfissionalResponsavel.AgendamentosRealizados = null;
                }
            }

            var agendamentoViewModel = _mapper.Map<AgendamentoViewModel>(agendamentoBd);

            var todosTiposAtendimento = await _tipoAtendimentoRepository.ObterTodos(); // Busca todos os tipos de atendimento

            agendamentoViewModel.TiposAtendimento = (from ta in todosTiposAtendimento
                                                     select new AgendamentoTipoAtendimentoViewModel
                                                     {
                                                         TipoAtendimentoId = ta.Id,
                                                         TipoAtendimento = _mapper.Map<TipoAtendimentoViewModel>(ta), // Mapeia o tipo de atendimento completo
                                                         IsSelected = tiposAtendimentoAtuaisDoAgendamento.Any(ata => ata.TipoAtendimentoId == ta.Id) // Verifica se já está selecionado
                                                     }).ToList();

            return View(agendamentoViewModel);
        }

        [HttpPost]
        public async Task<JsonResult> IncluirTiposAtendimento(AgendamentoViewModel agendamentoViewModel) // ViewModel atualizado
        {
            try
            {
                var tiposSelecionados = agendamentoViewModel.TiposAtendimento
                 .Where(atavm => atavm.IsSelected)
                 .Select(atavm => new AgendamentoTipoAtendimento { TipoAtendimentoId = atavm.TipoAtendimentoId, AgendamentoId = agendamentoViewModel.Id })
                 .ToList();


                var resultado = await _agendamentoService.Incluir(
                    _mapper.Map<Agendamento>(agendamentoViewModel)
                ); // Esta chamada Incluir(Agendamento) deve lidar com a atualização de suas listas

                if (!resultado.Valido)
                    return Json(new { resultado = "falha", mensagem = string.Join(" ", resultado.Mensagens) });

                TempData["success"] = "Tipos de Atendimento adicionados ao Agendamento com Sucesso!";
                return Json(new { resultado = "sucesso" });
            }
            catch (Exception e)
            {
                return Json(new { resultado = "falha", mensagem = string.Join(" ", e.Message) });
            }
        }
    }
}