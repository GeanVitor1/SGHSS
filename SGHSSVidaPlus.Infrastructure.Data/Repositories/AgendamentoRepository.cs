using Microsoft.EntityFrameworkCore;
using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams; // Namespace ajustado (sem underscore)
using SGHSSVidaPlus.Domain.Interfaces.Repository; // Namespace ajustado
using SGHSSVidaPlus.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq; // Necessário para .Any(), .Where(), .ToList() etc.
using System.Threading.Tasks;

namespace SGHSSVidaPlus.Infrastructure.Data.Repositories
{
    public class AgendamentoRepository : RepositoryBase<Agendamento>, IAgendamentoRepository
    {
        public AgendamentoRepository(HospitalDbContext context) : base(context) { }

        public async Task<List<Agendamento>> BuscarAgendamentos(AgendamentoParams parametros)
        {
            var query = _context.Agendamentos.AsQueryable(); // Assumindo que a DbSet é Agendamentos

            if (parametros.IncluirProfissional)
            {
                query = query.Include(a => a.ProfissionalResponsavel); // Supondo que a propriedade de navegação é ProfissionalResponsavel
            }

            if (parametros.IncluirPaciente)
            {
                query = query.Include(a => a.Paciente); // Supondo que a propriedade de navegação é Paciente
            }

            // Filtro por Id
            if (parametros.Id != 0)
                query = query.Where(a => a.Id == parametros.Id);

            // Filtro por Descricao
            if (!string.IsNullOrWhiteSpace(parametros.Descricao))
                query = query.Where(a => a.Descricao.Contains(parametros.Descricao));

            // Filtro por período de DataHoraAgendamento
            if (parametros.DataHoraAgendamentoInicio != DateTime.MinValue && parametros.DataHoraAgendamentoFim != DateTime.MinValue)
            {
                parametros.DataHoraAgendamentoFim = parametros.DataHoraAgendamentoFim.Date.AddDays(1).AddTicks(-1);
                query = query.Where(a => a.DataHoraAgendamento >= parametros.DataHoraAgendamentoInicio && a.DataHoraAgendamento <= parametros.DataHoraAgendamentoFim);
            }

            // Filtro por Local (se a propriedade 'Local' existir na entidade Agendamento)
            // Lembre-se que você pode ter removido 'Local' antes, então só inclua se a entidade 'Agendamento' tiver
            // if (!string.IsNullOrWhiteSpace(parametros.Local))
            //    query = query.Where(a => a.Local.Contains(parametros.Local));


            // Filtro por Encerrado e StatusAgendamento
            // No AgendamentoParams que me enviou, tínhamos 'Encerrado' (bool) e 'StatusAgendamento' (string).
            // Vamos priorizar o 'Encerrado' bool e usar 'StatusAgendamento' para flexibilidade.
            if (parametros.Encerrado) // Se true, filtra por agendamentos encerrados
                query = query.Where(a => a.Encerrado);
            else if (!string.IsNullOrWhiteSpace(parametros.StatusAgendamento)) // Se 'Encerrado' for false e StatusAgendamento tiver valor
            {
                if (parametros.StatusAgendamento.Equals("NãoEncerrado", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(a => !a.Encerrado);
                }
                // Adicione outros casos para 'StatusAgendamento' aqui, se existirem (ex: "Cancelado")
            }


            // Filtro por UsuarioInclusao
            if (!string.IsNullOrWhiteSpace(parametros.UsuarioInclusao))
                query = query.Where(a => a.UsuarioInclusao.Contains(parametros.UsuarioInclusao));

            // Filtro por UsuarioEncerramento
            if (!string.IsNullOrWhiteSpace(parametros.UsuarioEncerramento))
                query = query.Where(a => a.UsuarioEncerramento.Contains(parametros.UsuarioEncerramento));

            // Filtro por período de DataEncerramento
            if (parametros.DataEncerramentoInicio.HasValue && parametros.DataEncerramentoFim.HasValue)
            {
                parametros.DataEncerramentoFim = parametros.DataEncerramentoFim.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(a => a.DataEncerramento >= parametros.DataEncerramentoInicio && a.DataEncerramento <= parametros.DataEncerramentoFim);
            }

            // Filtro por ProfissionalResponsavelId
            if (parametros.ProfissionalResponsavelId != 0)
                query = query.Where(a => a.ProfissionalResponsavelId == parametros.ProfissionalResponsavelId);

            // Filtro por PacienteId (verifica se o paciente está na lista de PacientesAgendados)
            if (parametros.PacienteId != 0)
                query = query.Where(a => a.PacientesAgendados.Any(ap => ap.PacienteId == parametros.PacienteId));


            query = query.OrderByDescending(a => a.Id); // Ordem decrescente por Id

            // Paginação
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
                        // Atualiza propriedades de AgendamentoPaciente se já existir
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