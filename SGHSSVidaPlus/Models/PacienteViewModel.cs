using SGHSSVidaPlus.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace SGHSSVidaPlus.MVC.Models
{
    public class PacienteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É Necessário informar o nome completo.")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "É Necessário informar a data de nascimento.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime? DataNascimento { get; set; }

        [Required(ErrorMessage = "É Necessário informar o CPF.")]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "CPF inválido. Use o formato XXX.XXX.XXX-XX")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "É Necessário informar o endereço.")]
        public string Endereco { get; set; }

        [Required(ErrorMessage = "É Necessário informar o estado civil.")]
        [Display(Name = "Estado Civil")]
        public string EstadoCivil { get; set; }

        public bool Ativo { get; set; } = true; // Paciente cadastrado automaticamente é ativo

        // Campos para Login
        [Required(ErrorMessage = "É necessário informar o e-mail para o login.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "É necessário informar uma senha.")]
        [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres de comprimento.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        [Compare("Senha", ErrorMessage = "A senha e a confirmação de senha não coincidem.")]
        public string ConfirmarSenha { get; set; }

        // Novo campo para o tipo de consulta que o paciente deseja agendar
        [Display(Name = "Tipo de Consulta Desejada (Opcional)")]
        public string? TipoConsultaDesejada { get; set; }

        // Campos de auditoria (mantidos)
        public string? UsuarioInclusao { get; set; }
        public DateTime DataInclusao { get; set; }

        // Lista de contatos (já existente)
        public List<PacienteContatoViewModel> Contatos { get; set; } = new List<PacienteContatoViewModel>();
        // Lista de histórico (já existente)
        public List<HistoricoPacienteViewModel> Historico { get; set; } = new List<HistoricoPacienteViewModel>();

        // Propriedade para armazenar o UserId do Identity, para vincular o paciente ao usuário
        public string? UserId { get; set; }
    }
}