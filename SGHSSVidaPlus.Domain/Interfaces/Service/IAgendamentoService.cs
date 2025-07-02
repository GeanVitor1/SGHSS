using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams; // Namespace ajustado (sem underscore)
using System.Collections.Generic; // Necessário para List
using System.Threading.Tasks; // Necessário para Task

namespace SGHSSVidaPlus.Domain.Interfaces.Service
{
    // ANTES: ISelecaoService -> AGORA: IAgendamentoService
    public interface IAgendamentoService
    {
        Task<OperationResult> Incluir(Agendamento agendamento);
        // O método IncluirCandidatos foi adaptado para IncluirPacientesNoAgendamento
        Task<OperationResult> IncluirPacientesNoAgendamento(List<AgendamentoPaciente> pacientesAgendados, int agendamentoId);
        Task<OperationResult> Editar(Agendamento agendamento);
        Task<OperationResult> EncerrarAgendamento(int agendamentoId, string usuarioEncerramento); // <--- Primeira declaração

        Task<OperationResult> ReabrirAgendamento(int agendamentoId, string usuarioReabertura);
    }
}
