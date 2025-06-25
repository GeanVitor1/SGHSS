using SGHSSVidaPlus.Domain.Entities; // Namespace da entidade
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace SGHSSVidaPlus.MVC.Models // Namespace do projeto MVC
{
    public class PacienteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É Necessário informar o nome do paciente.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "É Necessário informar a data de nascimento.")]
        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }

        [Required(ErrorMessage = "É Necessário informar o CPF.")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "É Necessário informar o endereço.")]
        public string Endereco { get; set; }

        [Required(ErrorMessage = "É Necessário informar o estado civil.")]
        public string EstadoCivil { get; set; }

        public bool Ativo { get; set; }

        // CORREÇÃO AQUI: UsuarioInclusao como string? (anulável) para corresponder à entidade e tratar nulls
        public string? UsuarioInclusao { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataInclusao { get; set; }

        public List<PacienteContatoViewModel> Contatos { get; set; } = new List<PacienteContatoViewModel>();
        public List<HistoricoPacienteViewModel> Historico { get; set; } = new List<HistoricoPacienteViewModel>();
    }
}