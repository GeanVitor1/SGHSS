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
    public class AgendamentoRepository : RepositoryBase<Agendamento>, IAgendamentoRepository
    {
        public AgendamentoRepository(HospitalDbContext context) : base(context) { }

        public async Task<List<Agendamento>> BuscarAgendamentos(AgendamentoParams parametros)
        {
            var query = _context.Agendamentos.AsQueryable();

            if (parametros.IncluirProfissional)
            {
                query = query.Include(a => a.ProfissionalResponsavel);
            }

            if (parametros.IncluirPaciente)
            {
                query = query.Include(a => a.Paciente);
            }

            if (parametros.Id != 0)
                query = query.Where(a => a.Id == parametros.Id);

            if (!string.IsNullOrWhiteSpace(parametros.Descricao))
                query = query.Where(a => a.Descricao.Contains(parametros.Descricao));

            if (parametros.DataHoraAgendamentoInicio != DateTime.MinValue && parametros.DataHoraAgendamentoFim != DateTime.MinValue)
            {
                parametros.DataHoraAgendamentoFim = parametros.DataHoraAgendamentoFim.Date.AddDays(1).AddTicks(-1);
                query = query.Where(a => a.DataHoraAgendamento >= parametros.DataHoraAgendamentoInicio && a.DataHoraAgendamento <= parametros.DataHoraAgendamentoFim);
            }

            if (parametros.Encerrado)
                query = query.Where(a => a.Encerrado);
            else if (!string.IsNullOrWhiteSpace(parametros.StatusAgendamento))
            {
                if (parametros.StatusAgendamento.Equals("NãoEncerrado", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(a => !a.Encerrado);
                }
            }

            if (!string.IsNullOrWhiteSpace(parametros.UsuarioInclusao))
                query = query.Where(a => a.UsuarioInclusao.Contains(parametros.UsuarioInclusao));

            if (!string.IsNullOrWhiteSpace(parametros.UsuarioEncerramento))
                query = query.Where(a => a.UsuarioEncerramento.Contains(parametros.UsuarioEncerramento));

            if (parametros.DataEncerramentoInicio.HasValue && parametros.DataEncerramentoFim.HasValue)
            {
                parametros.DataEncerramentoFim = parametros.DataEncerramentoFim.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(a => a.DataEncerramento >= parametros.DataEncerramentoInicio && a.DataEncerramento <= parametros.DataEncerramentoFim);
            }

            if (parametros.ProfissionalResponsavelId != 0)
                query = query.Where(a => a.ProfissionalResponsavelId == parametros.ProfissionalResponsavelId);

            if (parametros.PacienteId != 0)
                query = query.Where(a => a.PacientesAgendados.Any(ap => ap.PacienteId == parametros.PacienteId));

            query = query.OrderByDescending(a => a.Id);

            if (parametros.TotalRegistros != 0)
                query = query.Take(parametros.TotalRegistros);

            return await query.ToListAsync();
        }

        public async Task IncluirPacientesNoAgendamento(List<AgendamentoPaciente> pacientesAgendados, int agendamentoId)
        {
            var agendamento = await _context.Agendamentos
                                            .Include(a => a.PacientesAgendados)
                                            .FirstOrDefaultAsync(a => a.Id == agendamentoId);

            if (agendamento != null)
            {
                var pacientesParaRemover = agendamento.PacientesAgendados
                    .Where(ap => !pacientesAgendados.Any(newAp => newAp.PacienteId == ap.PacienteId))
                    .ToList();

                if (pacientesParaRemover.Any())
                {
                    _context.AgendamentosPacientes.RemoveRange(pacientesParaRemover);
                }

                foreach (var novoPacienteAgendado in pacientesAgendados)
                {
                    if (!agendamento.PacientesAgendados.Any(ap => ap.PacienteId == novoPacienteAgendado.PacienteId))
                    {
                        novoPacienteAgendado.AgendamentoId = agendamentoId;
                        _context.AgendamentosPacientes.Add(novoPacienteAgendado);
                    }
                    else
                    {
                        var existente = agendamento.PacientesAgendados.FirstOrDefault(ap => ap.PacienteId == novoPacienteAgendado.PacienteId);
                        if (existente != null)
                        {
                            existente.Compareceu = novoPacienteAgendado.Compareceu;
                            existente.AtendimentoFinalizado = novoPacienteAgendado.AtendimentoFinalizado;
                            _context.Entry(existente).State = EntityState.Modified;
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
