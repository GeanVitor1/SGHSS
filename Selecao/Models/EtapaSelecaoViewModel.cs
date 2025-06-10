using System.ComponentModel.DataAnnotations;

namespace Selecao.MVC.Models
{
    public class EtapaSelecaoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É Necessário informar o nome do candidato")]
        public string Nome { get; set; }
        public bool Status { get; set; }
        public string UsuarioInclusao { get; set; }
        public DateTime DataInclusao { get; set; }
    }
}
