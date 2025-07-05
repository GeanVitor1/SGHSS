using SGHSSVidaPlus.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering; // Necessário para SelectListItem

namespace SGHSSVidaPlus.MVC.Models
{
    public class PacienteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É necessário informar o nome do paciente.")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "É necessário informar a data de nascimento.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime? DataNascimento { get; set; }

        [Required(ErrorMessage = "É necessário informar o CPF.")]
        [StringLength(14, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 ou 14 caracteres.")]
        [Display(Name = "CPF")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "É necessário informar o endereço.")]
        [Display(Name = "Endereço Completo")]
        public string Endereco { get; set; }

        [Required(ErrorMessage = "É necessário informar o estado civil.")]
        [Display(Name = "Estado Civil")]
        public string EstadoCivil { get; set; }

        public bool Ativo { get; set; }

        public string? UsuarioInclusao { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataInclusao { get; set; }

        // NOVO: Campos para o cadastro do usuário
        [Required(ErrorMessage = "É necessário informar o e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "É necessário informar a senha.")]
        [StringLength(100, ErrorMessage = "A {0} deve ter no mínimo {2} e no máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        [Compare("Senha", ErrorMessage = "A senha e a confirmação de senha não coincidem.")]
        public string ConfirmarSenha { get; set; }

        // NOVO: Campo para o tipo de consulta para agendamento automático
        [Display(Name = "Tipo de Consulta Desejada")]
        public string? TipoConsultaDesejada { get; set; }
        public List<SelectListItem>? TiposConsultaDisponiveis { get; set; } // Para popular um dropdown

        // NOVO: Propriedade para armazenar o ID do ApplicationUser, se já existir
        public string? ApplicationUserId { get; set; }

        public List<PacienteContatoViewModel> Contatos { get; set; } = new List<PacienteContatoViewModel>();
        public List<HistoricoPacienteViewModel> Historico { get; set; } = new List<HistoricoPacienteViewModel>();
    }

}