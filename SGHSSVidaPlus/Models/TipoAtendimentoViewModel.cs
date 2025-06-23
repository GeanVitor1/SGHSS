using System.ComponentModel.DataAnnotations;
using System; // Para DateTime

namespace SGHSSVidaPlus.MVC.Models // Namespace atualizado
{
    public class TipoAtendimentoViewModel // Nome da classe atualizado
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É Necessário informar o nome do tipo de atendimento")] // Mensagem atualizada
        public string Nome { get; set; }
        public bool Ativo { get; set; } // Substitui Status
        public string UsuarioInclusao { get; set; }
        public DateTime DataInclusao { get; set; }
    }
}