using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SGHSSVidaPlus.Domain.Entities
{
    public class Paciente
    {
        public int Id { get; set; }
        public string Nome { get; set; } // Nome é Required (no ViewModel), então não será null
        public DateTime DataNascimento { get; set; } // DataNascimento é Required (no ViewModel)
        public string CPF { get; set; } // CPF é Required (no ViewModel)
        public string Endereco { get; set; } // Endereco é Required (no ViewModel)
        public string EstadoCivil { get; set; } // EstadoCivil é Required (no ViewModel)
        public bool Ativo { get; set; }

        // CORREÇÃO: UsuarioInclusao JÁ ESTÁ string?, mas reconfirmando
        public string? UsuarioInclusao { get; set; }

        public DateTime DataInclusao { get; set; }

        public List<PacienteContato> Contatos { get; set; } = new List<PacienteContato>(); // Listas devem ser inicializadas
        public List<HistoricoPaciente> Historico { get; set; } = new List<HistoricoPaciente>(); // Listas devem ser inicializadas
        public List<AgendamentoPaciente> Agendamentos { get; set; } = new List<AgendamentoPaciente>(); // Listas devem ser inicializadas
    }

    // MANTIDO: PacienteContato (e suas props string)
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

    // MANTIDO: HistoricoPaciente (e suas props string)
    public class HistoricoPaciente
    {
        public int Id { get; set; }
        public string Titulo { get; set; } // O título geralmente é obrigatório.
        public string? Descricao { get; set; } // <-- MUDANÇA AQUI: Pode ser nulo no DB
        public DateTime? DataEvento { get; set; }
        public string? ProfissionalResponsavel { get; set; } // <-- MUDANÇA AQUI: Pode ser nulo no DB

        [JsonIgnore]
        public Paciente Paciente { get; set; }
        public int PacienteId { get; set; }
    }
}