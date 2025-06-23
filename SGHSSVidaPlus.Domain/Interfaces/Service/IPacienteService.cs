using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams; // Namespace ajustado (sem underscore)
using System.Threading.Tasks; // Necessário para Task

namespace SGHSSVidaPlus.Domain.Interfaces.Service
{
    // ANTES: ICandidatosService -> AGORA: IPacienteService
    public interface IPacienteService
    {
        Task<OperationResult> Incluir(Paciente paciente);
        Task<OperationResult> Editar(Paciente paciente);
        Task<OperationResult> AlterarStatus(Paciente paciente); // Para ativar/inativar o cadastro do paciente
    }
}