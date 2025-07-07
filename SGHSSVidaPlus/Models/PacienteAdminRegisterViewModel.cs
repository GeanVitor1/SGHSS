// SGHSSVidaPlus.MVC/Models/PacienteAdminRegisterViewModel.cs

using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SGHSSVidaPlus.MVC.Models
{
    // Este ViewModel será usado quando um ADMINISTRADOR CADASTRA um paciente.
    // Ele remove a obrigatoriedade de e-mail e senha, pois o admin pode não criar um login para o paciente no momento.
    public class PacienteAdminRegisterViewModel : PacienteViewModel
    {
        // Sobrescreve as propriedades de e-mail e senha para remover a obrigatoriedade
        // (Remove o [Required] e permite nulos)

        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [Display(Name = "E-mail")]
        public new string? Email { get; set; } // 'new' oculta o membro base, tornando-o anulável

        [StringLength(100, ErrorMessage = "A {0} deve ter no mínimo {2} e no máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public new string? Senha { get; set; } // 'new' oculta o membro base, tornando-o anulável

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        [Compare("Senha", ErrorMessage = "A senha e a confirmação de senha não coincidem.")]
        public new string? ConfirmarSenha { get; set; } // 'new' oculta o membro base, tornando-o anulável

        // Se o admin cadastrar um paciente, o ApplicationUserId pode ser nulo inicialmente.
        // Ele será preenchido se o admin decidir criar um usuário para o paciente depois, ou se o paciente se auto-cadastrar usando seu CPF já existente.
        public new string? ApplicationUserId { get; set; }
    }
}