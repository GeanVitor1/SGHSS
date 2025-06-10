using Selecao.Domain.Entities;
using Selecao.Domain.Extensions_Params;

namespace Selecao.Domain.Interfaces.Service
{
    public interface ISelecaoService
    {
        Task<OperationResult> Incluir(SelecaoCandidato selecao);

        Task<OperationResult> IncluirCandidatos(List<CandidatoSelecaoCandidato> candidatos, int selecaoid);
    }
}
