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
    public class SelecaoRepository : RepositoryBase<SelecaoCandidato>, ISelecaoRepository
    {
        public SelecaoRepository(SelecaoContext context) : base(context) { }
        public async Task<List<SelecaoCandidato>> BuscarSelecoes(SelecaoParams parametros)
        {
            var query = _context.Selecao.AsQueryable();

            if (parametros.InfoExtras)
            {
                query = query.Include(s => s.Candidatos)
                    .ThenInclude(c => c.Candidato)
                    .Include(s => s.Etapas)
                    .ThenInclude(e => e.Etapa);
            }

            if(parametros.Id != 0)
                query = query.Where(s => s.Id == parametros.Id);

            if (!string.IsNullOrWhiteSpace(parametros.Cargo))
                query = query.Where(s => s.Cargo.Contains(parametros.Cargo));



            query = query.OrderByDescending(s => s.Id);

            if (parametros.TotalRegistros != 0)
                query = query.Take(parametros.TotalRegistros);

            return query.ToList();
        }

        public async Task IncluirCandidatos(List<CandidatoSelecaoCandidato> candidatos, int selecaoid)
        {
            var selecao = (await BuscarSelecoes(new SelecaoParams { Id = selecaoid, InfoExtras = true })).FirstOrDefault();

            if (selecao.Candidatos.Any())
                _context.CandidatoSelecao.RemoveRange( selecao.Candidatos);

            await _context.CandidatoSelecao.AddRangeAsync(candidatos);

            await _context.SaveChangesAsync();
        }
    }
}
