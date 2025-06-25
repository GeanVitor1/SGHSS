using System;
using System.ComponentModel.DataAnnotations;

namespace SGHSSVidaPlus.MVC.Models
{
    public class HistoricoPacienteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É necessário informar o título do registro.")]
        public string Titulo { get; set; }

        public string? Descricao { get; set; } // <-- MUDANÇA AQUI: Deve corresponder à entidade

        [DataType(DataType.Date)]
        public DateTime? DataEvento { get; set; }

        public string? ProfissionalResponsavel { get; set; } // <-- MUDANÇA AQUI: Deve corresponder à entidade

        public int PacienteId { get; set; }
    }
}