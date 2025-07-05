using System.ComponentModel.DataAnnotations;

namespace SGHSSVidaPlus.MVC.Models
{

    public class PacienteContatoViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Informe o contato.")]
        public string Contato { get; set; }
        [Required(ErrorMessage = "Informe o tipo de contato.")]
        public string Tipo { get; set; }
        public bool IsWhatsApp { get; set; }
        public int PacienteId { get; set; }
    }
}