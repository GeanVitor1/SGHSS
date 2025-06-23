using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SGHSSVidaPlus.Domain.Entities
{

    public class Agendamento
    {
        public int Id { get; set; }
        public string Descricao { get; set; } 
        public DateTime DataHoraAgendamento { get; set; } 
        public bool Encerrado { get; set; } 
        public string UsuarioInclusao { get; set; } 
        public DateTime DataInclusao { get; set; }
        public string UsuarioEncerramento { get; set; } 
        public DateTime? DataEncerramento { get; set; } 

        public List<AgendamentoPaciente> PacientesAgendados { get; set; }

        public List<AgendamentoTipoAtendimento> TiposAtendimento { get; set; }

        public ProfissionalSaude ProfissionalResponsavel { get; set; }
        public int ProfissionalResponsavelId { get; set; }
    }

    public class AgendamentoPaciente
    {
        public int Id { get; set; }

        public Agendamento Agendamento { get; set; }
        public int AgendamentoId { get; set; }

        public Paciente Paciente { get; set; }
        public int PacienteId { get; set; }

        public bool Compareceu { get; set; } 
        public bool AtendimentoFinalizado { get; set; } 
    }
}