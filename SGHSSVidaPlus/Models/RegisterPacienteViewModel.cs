using System.ComponentModel.DataAnnotations;
using System;

namespace SGHSSVidaPlus.MVC.Models
{
    public class RegisterPacienteViewModel
    {
        [Required(ErrorMessage = "Informe seu nome completo.")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Informe seu e-mail.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Crie uma senha.")]
        [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres de comprimento.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        [Compare("Senha", ErrorMessage = "A senha e a confirmação de senha não coincidem.")]
        public string ConfirmarSenha { get; set; }

        [Required(ErrorMessage = "Informe seu endereço.")]
        [Display(Name = "Endereço Completo")]
        public string Endereco { get; set; }

        [Required(ErrorMessage = "Informe seu CPF.")]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "CPF inválido. Use o formato XXX.XXX.XXX-XX")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "Informe sua data de nascimento.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime? DataNascimento { get; set; }

        [Required(ErrorMessage = "Informe seu estado civil.")]
        [Display(Name = "Estado Civil")]
        public string EstadoCivil { get; set; }

        // Tipo de Consulta (opcional, para agendamento inicial)
        [Display(Name = "Qual tipo de consulta você busca? (Opcional)")]
        public string? TipoConsultaDesejada { get; set; }
    }
}