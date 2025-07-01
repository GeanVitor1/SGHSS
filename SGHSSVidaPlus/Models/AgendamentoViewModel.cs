using System.Collections.Generic;
using System; // Para DateTime
using System.ComponentModel.DataAnnotations; // Para DataAnnotations

namespace SGHSSVidaPlus.MVC.Models // Namespace atualizado
{
    public class AgendamentoViewModel // Nome da classe atualizado
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É Necessário informar a descrição do agendamento.")]
        public string Descricao { get; set; }

        public string? Observacoes { get; set; } // Tornando nullable se não for sempre obrigatório
        public string? Local { get; set; } // Tornando nullable se não for sempre obrigatório

        [Required(ErrorMessage = "É Necessário informar a data e hora do agendamento.")]
        [DataType(DataType.DateTime)] // Garante que o tipo de dado é DateTime para a UI
        public DateTime DataHoraAgendamento { get; set; }

        // Propriedade para o ID do paciente selecionado
        [Required(ErrorMessage = "É Necessário selecionar o paciente.")]
        public int PacienteId { get; set; }

        // Propriedade para o ID do profissional responsável selecionado
        [Required(ErrorMessage = "É Necessário selecionar o profissional responsável.")]
        public int ProfissionalResponsavelId { get; set; }

        // Propriedades de navegação (útil para exibir dados em formulários de visualização ou edição)
        // Não são usadas para input direto, mas para carregar dados relacionados.
        public ProfissionalSaudeViewModel? ProfissionalResponsavel { get; set; }
        public PacienteViewModel? Paciente { get; set; } // Adicionando propriedade de navegação para o Paciente principal do agendamento

        [Required(ErrorMessage = "É Necessário selecionar o status do agendamento.")]
        public string Status { get; set; } // Agendado, Confirmado, Cancelado, Realizado

        // Campos de auditoria/controle
        public bool Encerrado { get; set; }
        public string? UsuarioInclusao { get; set; } // Permitindo nulo, pois pode ser preenchido no Controller
        public DateTime DataInclusao { get; set; }
        public string? UsuarioEncerramento { get; set; } // Permitindo nulo
        public DateTime? DataEncerramento { get; set; } // Permitindo nulo

        // Listas de ViewModels aninhados (para múltiplos pacientes ou tipos de atendimento, se o agendamento for complexo)
        // Mantenho estas listas, assumindo que um agendamento pode ter múltiplos pacientes ou tipos de atendimento
        // A distinção é que o 'PacienteId' acima seria para o "paciente principal" do agendamento,
        // enquanto 'PacientesAgendados' seria para um grupo, se aplicável.
        // Se um agendamento for SEMPRE para UM paciente, a lista 'PacientesAgendados' pode ser removida
        // e o 'PacienteId' principal deve ser usado para todas as interações.
        public List<AgendamentoPacienteViewModel> PacientesAgendados { get; set; } = new List<AgendamentoPacienteViewModel>();
        public List<AgendamentoTipoAtendimentoViewModel> TiposAtendimento { get; set; } = new List<AgendamentoTipoAtendimentoViewModel>();
    }

    // ViewModel para a entidade de ligação Agendamento-Paciente
    public class AgendamentoPacienteViewModel
    {
        public int Id { get; set; }
        public int AgendamentoId { get; set; }
        public int PacienteId { get; set; }
        public PacienteViewModel? Paciente { get; set; } // Para carregar os dados do paciente
        public bool Compareceu { get; set; }
        public bool AtendimentoFinalizado { get; set; }
        public bool IsSelected { get; set; } // Usado para seleção em UI, se houver uma lista de seleção
    }

    // ViewModel para a entidade de ligação Agendamento-TipoAtendimento
    public class AgendamentoTipoAtendimentoViewModel
    {
        public int Id { get; set; }
        public int AgendamentoId { get; set; }
        public int TipoAtendimentoId { get; set; }
        public TipoAtendimentoViewModel? TipoAtendimento { get; set; } // Para carregar dados do tipo de atendimento
        public bool IsSelected { get; set; } // Usado para seleção em UI, se houver uma lista de seleção
    }

}