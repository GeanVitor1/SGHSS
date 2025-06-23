using System.ComponentModel.DataAnnotations;

namespace SGHSSVidaPlus.MVC.Models
{
    public class PacienteContatoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É necessário informar o contato.")]
        public string Contato { get; set; }

        [Required(ErrorMessage = "É necessário informar o tipo de contato (Ex: Celular, Email).")]
        public string Tipo { get; set; }
        public bool IsWhatsApp { get; set; }

        public int PacienteId { get; set; }
    }
}