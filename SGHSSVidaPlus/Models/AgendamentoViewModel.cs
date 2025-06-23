using System.Collections.Generic;
using System; // Para DateTime
using System.ComponentModel.DataAnnotations; // Para DataAnnotations

namespace SGHSSVidaPlus.MVC.Models // Namespace atualizado
{
    public class AgendamentoViewModel // Nome da classe atualizado
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É Necessário informar a descrição do agendamento")]
        public string Descricao { get; set; } // Substitui Cargo

        // Removido: Vagas (não se aplica a agendamento genérico)

        public string Observacoes { get; set; } // Substitui Objetivo
        public string Local { get; set; } // Adicionado, se aplicável

        [Required(ErrorMessage = "É Necessário informar a data e hora do agendamento")]
        public DateTime DataHoraAgendamento { get; set; } // Adicionado

        public bool Encerrado { get; set; }
        public string UsuarioInclusao { get; set; }
        public DateTime DataInclusao { get; set; }
        public string UsuarioEncerramento { get; set; }
        public DateTime? DataEncerramento { get; set; }

        public int ProfissionalResponsavelId { get; set; } // Adicionado para ligar ao profissional
        public ProfissionalSaudeViewModel ProfissionalResponsavel { get; set; } // Para exibir no ViewModel

        // Listas de ViewModels aninhados (baseados nas entidades de ligação)
        public List<AgendamentoPacienteViewModel> PacientesAgendados { get; set; } = new List<AgendamentoPacienteViewModel>(); // Substitui Candidatos
        public List<AgendamentoTipoAtendimentoViewModel> TiposAtendimento { get; set; } = new List<AgendamentoTipoAtendimentoViewModel>(); // Substitui Etapas
    }

    // Adaptação de CandidatoSelectViewModel para AgendamentoPacienteViewModel
    public class AgendamentoPacienteViewModel
    {
        public int Id { get; set; }
        public int AgendamentoId { get; set; }
        public int PacienteId { get; set; }
        public PacienteViewModel Paciente { get; set; } // Para carregar os dados do paciente
        public bool Compareceu { get; set; }
        public bool AtendimentoFinalizado { get; set; }
        public bool IsSelected { get; set; } // Se usado para seleção em UI
    }

    // Adaptação de EtapaSelectViewModel para AgendamentoTipoAtendimentoViewModel
    public class AgendamentoTipoAtendimentoViewModel
    {
        public int Id { get; set; }
        public int AgendamentoId { get; set; }
        public int TipoAtendimentoId { get; set; }
        public TipoAtendimentoViewModel TipoAtendimento { get; set; } // Para carregar dados do tipo de atendimento
        public bool IsSelected { get; set; } // Se usado para seleção em UI
    }
}