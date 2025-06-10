using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Selecao.MVC.Models
{
    public class UsuarioViewModel
    {
        [Required(ErrorMessage = "Informe o nome")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Informe o login")]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Informe o email")]
        [EmailAddress(ErrorMessage = "Informe um email válido")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string Password { get; set; }

        [Required(ErrorMessage = "Informe a confirmação da senha")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmação")]
        [Compare("Password", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmPassword { get; set; }
        public bool Bloqueado { get; set; }
        
        public bool Admin {  get; set; }

        public List<ClaimViewModel> Permissoes { get; set; }
    }
}
