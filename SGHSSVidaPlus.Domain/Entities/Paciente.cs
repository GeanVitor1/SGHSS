using System; 
using System.Collections.Generic; 
using System.Text.Json.Serialization;

namespace SGHSSVidaPlus.Domain.Entities
{
    public class Paciente
    {
        // Propriedades básicas do paciente
        public int Id { get; set; }
        public string Nome { get; set; } 
        public DateTime DataNascimento { get; set; }
        public string CPF { get; set; } 
        public string Endereco { get; set; } 
        public string EstadoCivil { get; set; } 
        public bool Ativo { get; set; } 
        public string UsuarioInclusao { get; set; } 
        public DateTime DataInclusao { get; set; }

        public List<PacienteContato> Contatos { get; set; }
        public List<HistoricoPaciente> Historico { get; set; }
        public List<AgendamentoPaciente> Agendamentos { get; set; }

    }

    public class PacienteContato
    {
        public int Id { get; set; }
        public string Contato { get; set; } 
        public string Tipo { get; set; } 

        [JsonIgnore] 
        public Paciente Paciente { get; set; }
        public int PacienteId { get; set; }
    }

    public class HistoricoPaciente
    {
        public int Id { get; set; }
        public string Titulo { get; set; } 
        public string Descricao { get; set; }
        public DateTime? DataEvento { get; set; } 
        public string ProfissionalResponsavel { get; set; }

        [JsonIgnore]
        public Paciente Paciente { get; set; }
        public int PacienteId { get; set; }
    }
}