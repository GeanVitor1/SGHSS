using System.ComponentModel.DataAnnotations;
using System; // Para DateTime

namespace SGHSSVidaPlus.MVC.Models
{
    public class TipoAtendimentoViewModel
    {
        public int Id { get; set; }
        public int AgendamentoId { get; set; }
        public int TipoAtendimentoId { get; set; }
        public string Nome { get; set; } // Provavelmente você precisa desta propriedade também, já que usa Model[i].TipoAtendimento.Nome
        public bool Ativo { get; set; } // <-- Adicione esta linha!
        public TipoAtendimentoViewModel? TipoAtendimento { get; set; } // Pode ser redundante ou indicar uma estrutura complexa
        public bool IsSelected { get; set; }
    }
}