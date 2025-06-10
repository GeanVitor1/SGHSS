using Selecao.Domain.Entities;
namespace Selecao.Domain.Interfaces.Repository
{
    public interface ICandidatosRepository : IRepositoryBase<Candidato>
    {

        Task<List<Candidato>> BuscarCandidatos(CandidatoParams parametros);
        Task Editar(Candidato candidato);
    }
}
