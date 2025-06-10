using Selecao.Domain.Entities;
using Selecao.Domain.Extensions_Params;

namespace Selecao.Domain.Interfaces.Service
{
    public interface ICandidatosService
    {
        Task<OperationResult> Incluir(Candidato candidato);
        Task<OperationResult> Editar(Candidato candidato);
        Task<OperationResult> AlterarStatus(Candidato candidato);
    }
}
