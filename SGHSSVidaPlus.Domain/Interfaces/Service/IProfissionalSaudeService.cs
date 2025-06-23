using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams; // Namespace ajustado (sem underscore)
using System.Threading.Tasks; // Necessário para Task

namespace SGHSSVidaPlus.Domain.Interfaces.Service
{
    // NOVA INTERFACE: IProfissionalSaudeService
    public interface IProfissionalSaudeService
    {
        Task<OperationResult> Incluir(ProfissionalSaude profissionalSaude);
        Task<OperationResult> Editar(ProfissionalSaude profissionalSaude);
        Task<OperationResult> AlterarStatus(ProfissionalSaude profissionalSaude); // Para ativar/inativar o profissional
    }
}