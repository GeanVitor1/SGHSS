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
    public class PacienteRepository : RepositoryBase<Paciente>, IPacienteRepository
    {
        public PacienteRepository(HospitalDbContext context) : base(context) { }

        public async Task<List<Paciente>> BuscarPacientes(PacienteParams parametros)
        {
            var query = _context.Pacientes.AsQueryable();

            // CORREÇÃO CRÍTICA AQUI: ADICIONANDO OS INCLUDES
            if (parametros.IncluirContatosHistorico)
            {
                query = query
                    .Include(p => p.Contatos)
                    .Include(p => p.Historico);
            }

            if (parametros.Id != 0)
                query = query.Where(p => p.Id == parametros.Id);

            if (!string.IsNullOrWhiteSpace(parametros.Nome))
                query = query.Where(p => p.Nome.Contains(parametros.Nome));

            if (parametros.DataInclusaoInicio != DateTime.MinValue && parametros.DataInclusaoFim != DateTime.MinValue)
            {
                parametros.DataInclusaoFim = parametros.DataInclusaoFim.Date.AddDays(1).AddTicks(-1);
                query = query.Where(p => p.DataInclusao >= parametros.DataInclusaoInicio && p.DataInclusao <= parametros.DataInclusaoFim);
            }

            if (parametros.DataNascimentoInicio != DateTime.MinValue && parametros.DataNascimentoFim != DateTime.MinValue)
            {
                parametros.DataNascimentoFim = parametros.DataNascimentoFim.Date.AddDays(1).AddTicks(-1);
                query = query.Where(p => p.DataNascimento >= parametros.DataNascimentoInicio && parametros.DataNascimento <= parametros.DataNascimentoFim);
            }

            if (!string.IsNullOrWhiteSpace(parametros.Endereco))
                query = query.Where(p => p.Endereco.Contains(parametros.Endereco));

            if (!string.IsNullOrWhiteSpace(parametros.EstadoCivil))
                query = query.Where(p => p.EstadoCivil == parametros.EstadoCivil);

            if (!string.IsNullOrWhiteSpace(parametros.UsuarioInclusao))
                query = query.Where(p => p.UsuarioInclusao.Contains(parametros.UsuarioInclusao));

            if (parametros.Ativo.HasValue)
            {
                query = query.Where(p => p.Ativo == parametros.Ativo.Value);
            }

            query = query.OrderByDescending(p => p.Id);

            if (parametros.TotalRegistros != 0)
                query = query.Take(parametros.TotalRegistros);

            return await query.ToListAsync();
        }
    }
}