using Selecao.Domain.Entities;
using Selecao.Domain.Extensions_Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selecao.Domain.Interfaces.Repository
{
    public interface IEtapaSelecaoRepository : IRepositoryBase<EtapaSelecao> 
    {
        Task<List<EtapaSelecao>> BuscarEtapas(EtapaSelecaoParams parametros);
    }
}
