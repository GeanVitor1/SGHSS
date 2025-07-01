using System;
using System.Collections.Generic;

namespace SGHSSVidaPlus.Domain.Entities
{
    public class Agendamento
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string? Observacoes { get; set; }
        public string? Local { get; set; }
        public DateTime DataHoraAgendamento { get; set; }
        public bool Encerrado { get; set; }
        public string? UsuarioInclusao { get; set; }
        public DateTime DataInclusao { get; set; }
        public string? UsuarioEncerramento { get; set; }
        public DateTime? DataEncerramento { get; set; }
        public string Status { get; set; }

        public int ProfissionalResponsavelId { get; set; }
        public ProfissionalSaude ProfissionalResponsavel { get; set; } // Propriedade de navegação

        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; } // Propriedade de navegação

        public ICollection<AgendamentoPaciente> PacientesAgendados { get; set; } = new List<AgendamentoPaciente>();
        public ICollection<AgendamentoTipoAtendimento> TiposAtendimento { get; set; } = new List<AgendamentoTipoAtendimento>();
    }


    public class AgendamentoPaciente
    {
        public int Id { get; set; }
        public int AgendamentoId { get; set; }
        public Agendamento Agendamento { get; set; }
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public bool Compareceu { get; set; }
        public bool AtendimentoFinalizado { get; set; }
    }

}