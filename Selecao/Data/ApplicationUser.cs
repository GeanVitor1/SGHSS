using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Selecao.MVC.Data
{
    public class ApplicationUser: IdentityUser
    {
        [PersonalData]
        public bool Bloqueado {  get; set; }

        [PersonalData]
        [Column(TypeName = "varchar(150)")]
        public string Nome { get; set; } = string.Empty;
    }
}
