using Selecao.Domain.Entities;
using Selecao.Domain.Extensions_Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selecao.Domain.Interfaces.Repository
{
    public interface ISelecaoRepository : IRepositoryBase<SelecaoCandidato>
    {
        Task<List<SelecaoCandidato>> BuscarSelecoes(SelecaoParams parametros);

        Task IncluirCandidatos(List<CandidatoSelecaoCandidato> candidatos, int selecaoid);
    }
}
