using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Necessário para SelectListItem

namespace SGHSSVidaPlus.MVC.Models
{
    public class AgendamentoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É necessário informar a descrição do agendamento.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "É necessário informar a data e hora do agendamento.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Data e Hora do Agendamento")]
        public DateTime DataHoraAgendamento { get; set; }

        [Display(Name = "Profissional Responsável")]
        public int? ProfissionalResponsavelId { get; set; }

        // Propriedade para exibir o nome do profissional. Útil para preenchimento direto.
        // Já existia, mantida.
        [Display(Name = "Nome do Profissional")]
        public string? ProfissionalResponsavelNome { get; set; }

        // RESTAURADO: Propriedade de navegação para o ViewModel do Profissional de Saúde
        // Isso permite acessar propriedades como Model.ProfissionalResponsavel.Nome
        public ProfissionalSaudeViewModel? ProfissionalResponsavel { get; set; }

        [Display(Name = "Local de Atendimento")]
        public string? Local { get; set; }

        [Required(ErrorMessage = "É necessário informar o paciente.")]
        public int PacienteId { get; set; }

        // Propriedade para exibir o nome do paciente. Útil para preenchimento direto.
        // Já existia, mantida.
        [Display(Name = "Nome do Paciente")]
        public string? PacienteNome { get; set; }

        // RESTAURADO: Propriedade de navegação para o ViewModel do Paciente
        // Isso permite acessar propriedades como Model.Paciente.Nome, .CPF, .DataNascimento
        public PacienteViewModel? Paciente { get; set; }


        [Required(ErrorMessage = "É necessário informar o status do agendamento.")]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        public bool Encerrado { get; set; }
        public string? UsuarioEncerramento { get; set; }
        public DateTime? DataEncerramento { get; set; }

        public string? UsuarioInclusao { get; set; }
        public DateTime DataInclusao { get; set; }

        // Mantenha AgendamentoPacienteViewModel, pois parece que você tem uma lista
        // de pacientes associados ao agendamento (muitos para muitos) para alguma funcionalidade.
        public List<AgendamentoPacienteViewModel> PacientesAgendados { get; set; } = new List<AgendamentoPacienteViewModel>();
    }

    // ViewModel para a entidade de ligação Agendamento-Paciente
    // Esta classe está correta como você a definiu.
    public class AgendamentoPacienteViewModel
    {
        public int Id { get; set; } // Id da entidade AgendamentoPaciente

        [Display(Name = "ID do Agendamento")]
        public int AgendamentoId { get; set; }

        [Display(Name = "ID do Paciente")]
        public int PacienteId { get; set; }

        // Referência ao ViewModel do Paciente para exibir os dados do paciente agendado
        public PacienteViewModel? Paciente { get; set; }

        [Display(Name = "Compareceu")]
        public bool Compareceu { get; set; }

        [Display(Name = "Atendimento Finalizado")]
        public bool AtendimentoFinalizado { get; set; }

        public bool IsSelected { get; set; } // Usado para seleção em UI, se houver uma lista de seleção
    }
}