using Microsoft.EntityFrameworkCore;
using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams;
using SGHSSVidaPlus.Domain.Interfaces.Repository;
using SGHSSVidaPlus.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGHSSVidaPlus.Infrastructure.Data.Repositories
{
    public class ProfissionalSaudeRepository : RepositoryBase<ProfissionalSaude>, IProfissionalSaudeRepository
    {
        public ProfissionalSaudeRepository(HospitalDbContext context) : base(context) { }

        public async Task<List<ProfissionalSaude>> BuscarProfissional(ProfissionalSaudeParams parametros)
        {
            var query = _context.ProfissionalSaude.AsQueryable();

            if (parametros.IncluirFormacaoCursos)
            {
                query = query.Include(ps => ps.Formacao)
                             .Include(ps => ps.Cursos);
            }

            if (parametros.Id != 0)
                query = query.Where(ps => ps.Id == parametros.Id);

            if (!string.IsNullOrWhiteSpace(parametros.Nome))
                query = query.Where(ps => ps.Nome.Contains(parametros.Nome));

            if (!string.IsNullOrWhiteSpace(parametros.Cargo))
                query = query.Where(ps => ps.Cargo.Contains(parametros.Cargo));

            if (!string.IsNullOrWhiteSpace(parametros.Telefone))
                query = query.Where(ps => ps.Telefone.Contains(parametros.Telefone));

            if (!string.IsNullOrWhiteSpace(parametros.Email))
                query = query.Where(ps => ps.Email.Contains(parametros.Email));

            if (parametros.Ativo) // Filtra por profissionais ativos
                query = query.Where(ps => ps.Ativo);
            else if (!parametros.Ativo && parametros.Id == 0 && string.IsNullOrWhiteSpace(parametros.Nome) && // Adicionei condição para não filtrar se for default false e não houver outros filtros
                     string.IsNullOrWhiteSpace(parametros.Cargo) && string.IsNullOrWhiteSpace(parametros.Telefone) &&
                     string.IsNullOrWhiteSpace(parametros.Email) && string.IsNullOrWhiteSpace(parametros.UsuarioInclusao) &&
                     parametros.DataInclusaoInicio == DateTime.MinValue && parametros.DataInclusaoFim == DateTime.MinValue &&
                     parametros.TotalRegistros == 10) // Basicamente, se só 'Ativo' é false e nada mais, talvez não filtrar por inativos
            {
                // Não faz nada, para retornar todos se 'Ativo' for false e não houver outros filtros.
                // Ajuste esta lógica de filtro conforme sua necessidade exata para 'Ativo'.
            }


            if (!string.IsNullOrWhiteSpace(parametros.UsuarioInclusao))
                query = query.Where(ps => ps.UsuarioInclusao.Contains(parametros.UsuarioInclusao));

            if (parametros.DataInclusaoInicio != DateTime.MinValue && parametros.DataInclusaoFim != DateTime.MinValue)
            {
                parametros.DataInclusaoFim = parametros.DataInclusaoFim.Date.AddDays(1).AddTicks(-1);
                query = query.Where(ps => ps.DataInclusao >= parametros.DataInclusaoInicio && ps.DataInclusao <= parametros.DataInclusaoFim);
            }

            query = query.OrderByDescending(ps => ps.Id);

            if (parametros.TotalRegistros != 0)
                query = query.Take(parametros.TotalRegistros);

            return await query.ToListAsync();
        }

        public async Task<ProfissionalSaude> ObterProfissionalComColecoes(int id)
        {
            return await _context.ProfissionalSaude
           .Include(ps => ps.Formacao)
           .Include(ps => ps.Cursos)
           .FirstOrDefaultAsync(ps => ps.Id == id);
        }
    }
}