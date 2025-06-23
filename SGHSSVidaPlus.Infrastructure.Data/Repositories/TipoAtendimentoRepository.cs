using Microsoft.EntityFrameworkCore;
using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams; // Namespace ajustado (sem underscore)
using SGHSSVidaPlus.Domain.Interfaces.Repository; // Namespace ajustado
using SGHSSVidaPlus.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGHSSVidaPlus.Infrastructure.Data.Repositories
{
    // ANTES: EtapasSelecaoRepository -> AGORA: TipoAtendimentoRepository
    public class TipoAtendimentoRepository : RepositoryBase<TipoAtendimento>, ITipoAtendimentoRepository
    {
        public TipoAtendimentoRepository(HospitalDbContext context) : base(context) { }

        public async Task<List<TipoAtendimento>> BuscarTiposAtendimento(TipoAtendimentoParams parametros)
        {
            var query = _context.TiposAtendimento.AsQueryable(); // DbSet ajustado

            if (parametros.Id != 0)
                query = query.Where(e => e.Id == parametros.Id);

            if (!string.IsNullOrWhiteSpace(parametros.Nome))
                query = query.Where(e => e.Nome.Contains(parametros.Nome));

            // A linha 'query = query.Include(e => e.Status);' não faz sentido aqui,
            // pois 'Status' é uma propriedade booleana, não uma navegação.
            // O filtro já é feito abaixo.

            switch (parametros.StatusAtivoString) // Parâmetro ajustado
            {
                case "1": // Geralmente significa Ativo
                    query = query.Where(e => e.Ativo); // Propriedade ajustada
                    break;

                case "2": // Geralmente significa Inativo
                    query = query.Where(e => !e.Ativo); // Propriedade ajustada
                    break;
            }

            // O parametro 'Status' booleano pode ser usado para um filtro direto
            if (parametros.Ativo)
            {
                query = query.Where(e => e.Ativo);
            }
            // Se precisar filtrar por inativos quando 'Ativo' é false E 'StatusAtivoString' não foi usado,
            // precisaria de uma lógica mais específica. Mantendo a simplicidade, se Ativo for true, filtra por ativo.


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