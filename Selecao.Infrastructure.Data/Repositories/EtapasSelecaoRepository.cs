using Microsoft.EntityFrameworkCore;
using Selecao.Domain.Entities;
using Selecao.Domain.Extensions_Params;
using Selecao.Domain.Interfaces.Repository;
using Selecao.Infrastructure.Data.Context;
using Selecao.Infrastructure.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selecao.Infrastructure.Data.Repositories
{
    public class EtapasSelecaoRepository : RepositoryBase<EtapaSelecao>, IEtapaSelecaoRepository
    {
        public EtapasSelecaoRepository(SelecaoContext context) : base(context) { }

        public async Task<List<EtapaSelecao>> BuscarEtapas(EtapaSelecaoParams parametros)
        {
            var query = _context.EtapasSelecao.AsQueryable();

            if (parametros.Id != 0)
                query = query.Where(e => e.Id == parametros.Id);

            if (!string.IsNullOrWhiteSpace(parametros.Nome))
                query = query.Where(e => e.Nome.Contains(parametros.Nome));

            if (parametros.Status)
                query = query.Include(e => e.Status);


            switch (parametros.StatusString)
            {
                case "1":
                    query = query.Where(e => e.Status);
                    break;

                case "2":
                    query = query.Where(e => !e.Status);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(parametros.UsuarioInclusao))
                query = query.Where(e => e.UsuarioInclusao.Contains(parametros.UsuarioInclusao));

            if (parametros.DataInclusaoInicio != DateTime.MinValue && parametros.DataInclusaoFim != DateTime.MinValue)
            {
                parametros.DataInclusaoFim = parametros.DataInclusaoFim.Date.AddDays(1).AddTicks(-1);
                query = query.Where(c => c.DataInclusao >= parametros.DataInclusaoInicio && c.DataInclusao <= parametros.DataInclusaoFim);
            }

            query = query.OrderByDescending(c => c.Id);

            if (parametros.TotalRegistros != 0)
                query = query.Take(parametros.TotalRegistros);

            return await query.ToListAsync();
        }
    }
}

