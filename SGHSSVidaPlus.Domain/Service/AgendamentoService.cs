using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams;
using SGHSSVidaPlus.Domain.Interfaces.Repository;
using SGHSSVidaPlus.Domain.Interfaces.Service;
using System; // Para DateTime
using System.Collections.Generic;
using System.Linq; // Para usar métodos Linq como Any()
using System.Threading.Tasks;

namespace SGHSSVidaPlus.Application.Services
{
    public class AgendamentoService : IAgendamentoService
    {
        private readonly IAgendamentoRepository _agendamentoRepository;
        private readonly IPacienteRepository _pacienteRepository; // Para validar se o paciente existe
        private readonly IProfissionalSaudeRepository _profissionalSaudeRepository; // Para validar se o profissional existe

        public AgendamentoService(
            IAgendamentoRepository agendamentoRepository,
            IPacienteRepository pacienteRepository,
            IProfissionalSaudeRepository profissionalSaudeRepository)
        {
            _agendamentoRepository = agendamentoRepository;
            _pacienteRepository = pacienteRepository;
            _profissionalSaudeRepository = profissionalSaudeRepository;
        }

        // AgendamentoService.cs - Método Incluir
        public async Task<OperationResult> Incluir(Agendamento agendamento)
        {
            var result = new OperationResult();

            // ... outras validações

            // Valida se o PacienteId principal foi informado
            if (agendamento.PacienteId <= 0)
            {
                result.Valido = false;
                result.Mensagens.Add("O agendamento deve ter um paciente associado.");
            }
            else
            {
                var paciente = await _pacienteRepository.ObterPorId(agendamento.PacienteId);
                if (paciente == null)
                {
                    result.Valido = false;
                    result.Mensagens.Add($"Paciente com ID {agendamento.PacienteId} não encontrado.");
                }
            }

            if (result.Valido)
            {
                agendamento.DataInclusao = DateTime.Now;
                agendamento.UsuarioInclusao = "Sistema"; // Substituir pelo usuário real
                agendamento.Encerrado = false; // Agendamento começa como não encerrado

                await _agendamentoRepository.Incluir(agendamento);
                result.Mensagens.Add("Agendamento incluído com sucesso.");
            }

            return result;
        }

        public async Task<OperationResult> ReabrirAgendamento(int agendamentoId, string usuarioReabertura)
        {
            var result = new OperationResult();

            if (agendamentoId <= 0)
            {
                result.Valido = false;
                result.Mensagens.Add("ID do agendamento inválido.");
            }

            if (!result.Valido)
            {
                return result;
            }

            var agendamentoExistente = await _agendamentoRepository.ObterPorId(agendamentoId);
            if (agendamentoExistente == null)
            {
                result.Valido = false;
                result.Mensagens.Add("Agendamento não encontrado.");
                return result;
            }

            if (!agendamentoExistente.Encerrado)
            {
                result.Valido = false;
                result.Mensagens.Add("Agendamento não está encerrado para ser reaberto.");
                return result;
            }

            agendamentoExistente.Encerrado = false; // Define como não encerrado
            agendamentoExistente.DataEncerramento = null; // Limpa a data de encerramento
            agendamentoExistente.UsuarioEncerramento = null; // Limpa o usuário de encerramento

            await _agendamentoRepository.Alterar(agendamentoExistente);
            result.Mensagens.Add("Agendamento reaberto com sucesso.");

            return result;
        }

        // AgendamentoService.cs - Método Editar
        public async Task<OperationResult> Editar(Agendamento agendamento)
        {
            var result = new OperationResult();

            if (agendamento.Id <= 0)
            {
                result.Valido = false;
                result.Mensagens.Add("ID do agendamento inválido para edição.");
            }

            // Valida se o PacienteId principal foi informado
            if (agendamento.PacienteId <= 0)
            {
                result.Valido = false;
                result.Mensagens.Add("O agendamento deve ter um paciente associado.");
            }
            else
            {
                var paciente = await _pacienteRepository.ObterPorId(agendamento.PacienteId);
                if (paciente == null)
                {
                    result.Valido = false;
                    result.Mensagens.Add($"Paciente com ID {agendamento.PacienteId} não encontrado.");
                }
            }

            // ... outras validações (ProfissionalResponsavelId, etc.)

            if (!result.Valido)
            {
                return result;
            }

            var agendamentoExistente = await _agendamentoRepository.ObterPorId(agendamento.Id);
            if (agendamentoExistente == null)
            {
                result.Valido = false;
                result.Mensagens.Add("Agendamento não encontrado para edição.");
                return result;
            }

            // Atualiza as propriedades do agendamento existente
            agendamentoExistente.Descricao = agendamento.Descricao;
            agendamentoExistente.DataHoraAgendamento = agendamento.DataHoraAgendamento;
            agendamentoExistente.Local = agendamento.Local; // Se existir
            agendamentoExistente.ProfissionalResponsavelId = agendamento.ProfissionalResponsavelId;
            agendamentoExistente.PacienteId = agendamento.PacienteId; // Atualiza o ID do paciente principal
            agendamentoExistente.Observacoes = agendamento.Observacoes;

            await _agendamentoRepository.Alterar(agendamentoExistente);
            result.Mensagens.Add("Agendamento editado com sucesso.");

            return result;
        }

        public async Task<OperationResult> IncluirPacientesNoAgendamento(List<AgendamentoPaciente> pacientesAgendados, int agendamentoId)
        {
            var result = new OperationResult();

            if (agendamentoId <= 0)
            {
                result.Valido = false;
                result.Mensagens.Add("ID do agendamento inválido.");
            }
            if (pacientesAgendados == null || !pacientesAgendados.Any())
            {
                result.Valido = false;
                result.Mensagens.Add("Nenhum paciente fornecido para inclusão no agendamento.");
            }

            var agendamentoExistente = await _agendamentoRepository.ObterPorId(agendamentoId);
            if (agendamentoExistente == null)
            {
                result.Valido = false;
                result.Mensagens.Add("Agendamento não encontrado.");
                return result;
            }

            // Valida se os pacientes existem e associa ao agendamento
            foreach (var ap in pacientesAgendados)
            {
                var paciente = await _pacienteRepository.ObterPorId(ap.PacienteId);
                if (paciente == null)
                {
                    result.Valido = false;
                    result.Mensagens.Add($"Paciente com ID {ap.PacienteId} não encontrado.");
                    continue; // Continua para verificar outros pacientes, mas marca como inválido
                }
                ap.AgendamentoId = agendamentoId; // Garante que o AgendamentoId está setado corretamente
            }

            if (result.Valido)
            {
                await _agendamentoRepository.IncluirPacientesNoAgendamento(pacientesAgendados, agendamentoId);
                result.Mensagens.Add("Pacientes adicionados ao agendamento com sucesso.");
            }

            return result;
        }

       

        public async Task<OperationResult> EncerrarAgendamento(int agendamentoId, string usuarioEncerramento)
        {
            var result = new OperationResult();

            if (agendamentoId <= 0)
            {
                result.Valido = false;
                result.Mensagens.Add("ID do agendamento inválido.");
            }

            if (result.Valido)
            {
                var agendamentoExistente = await _agendamentoRepository.ObterPorId(agendamentoId);
                if (agendamentoExistente == null)
                {
                    result.Valido = false;
                    result.Mensagens.Add("Agendamento não encontrado.");
                    return result;
                }

                if (agendamentoExistente.Encerrado)
                {
                    result.Valido = false;
                    result.Mensagens.Add("Agendamento já está encerrado.");
                    return result;
                }

                agendamentoExistente.Encerrado = true;
                agendamentoExistente.DataEncerramento = DateTime.Now;
                agendamentoExistente.UsuarioEncerramento = usuarioEncerramento;

                await _agendamentoRepository.Alterar(agendamentoExistente);
                result.Mensagens.Add("Agendamento encerrado com sucesso.");
            }

            return result;
        }

    }
}