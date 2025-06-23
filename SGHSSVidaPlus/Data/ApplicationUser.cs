using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
// Nao adicione System.Collections.Generic aqui para Permissoes. Listas de ViewModels ficam nos ViewModels.

namespace SGHSSVidaPlus.MVC.Data // <-- ESTE NAMESPACE DEVE ESTAR CORRETO AQUI
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public bool Bloqueado { get; set; }

        [PersonalData]
        [Column(TypeName = "varchar(150)")]
        public string Nome { get; set; } = string.Empty;

        // Propriedade 'Admin' adicionada ao ApplicationUser
        [PersonalData]
        public bool Admin { get; set; }
    }
}