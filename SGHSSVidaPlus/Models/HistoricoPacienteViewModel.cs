using System;
using System.ComponentModel.DataAnnotations;

namespace SGHSSVidaPlus.MVC.Models
{
    public class HistoricoPacienteViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O título do registro é obrigatório.")]
        public string Titulo { get; set; }
        public string? Descricao { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DataEvento { get; set; }
        public string? ProfissionalResponsavel { get; set; }
        public int PacienteId { get; set; }
    }
}