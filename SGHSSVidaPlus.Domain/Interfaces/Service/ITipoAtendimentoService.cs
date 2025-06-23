using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams; // Namespace ajustado (sem underscore)
using System.Threading.Tasks; // Necessário para Task

namespace SGHSSVidaPlus.Domain.Interfaces.Service
{
    // ANTES: IEtapasSelecaoService -> AGORA: ITipoAtendimentoService
    public interface ITipoAtendimentoService
    {
        Task<OperationResult> Incluir(TipoAtendimento tipoAtendimento);
        Task<OperationResult> Editar(TipoAtendimento tipoAtendimento);
        Task<OperationResult> AlterarStatus(TipoAtendimento tipoAtendimento); // Para ativar/inativar um tipo de atendimento
    }
}