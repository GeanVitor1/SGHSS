using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SGHSSVidaPlus.Domain.Entities
{
    public class Paciente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string CPF { get; set; }
        public string Endereco { get; set; }
        public string EstadoCivil { get; set; }
        public bool Ativo { get; set; }

        public string? UsuarioInclusao { get; set; }
        public DateTime DataInclusao { get; set; }

        // NOVO: Propriedade para o ID do ApplicationUser associado
        public string? ApplicationUserId { get; set; }

        //public ApplicationUser? ApplicationUser { get; set; } // Propriedade de navegação

        public List<PacienteContato> Contatos { get; set; } = new List<PacienteContato>();
        public List<HistoricoPaciente> Historico { get; set; } = new List<HistoricoPaciente>();
        public List<AgendamentoPaciente> Agendamentos { get; set; } = new List<AgendamentoPaciente>();
    }

    public class PacienteContato
    {
        public int Id { get; set; }
        public string Contato { get; set; }
        public string Tipo { get; set; }
        public bool IsWhatsApp { get; set; }

        [JsonIgnore]
        public Paciente Paciente { get; set; }
        public int PacienteId { get; set; }
    }

    public class HistoricoPaciente
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string? Descricao { get; set; }
        public DateTime? DataEvento { get; set; }
        public string? ProfissionalResponsavel { get; set; }

        [JsonIgnore]
        public Paciente Paciente { get; set; }
        public int PacienteId { get; set; }
    }
}