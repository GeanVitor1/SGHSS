using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams;
using SGHSSVidaPlus.Domain.Interfaces.Repository; // Necessário para IRepositoryBase
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGHSSVidaPlus.Domain.Interfaces.Repository
{
    public interface ITipoAtendimentoRepository : IRepositoryBase<TipoAtendimento>
    {
        Task<List<TipoAtendimento>> BuscarTiposAtendimento(TipoAtendimentoParams parametros);
    }
}