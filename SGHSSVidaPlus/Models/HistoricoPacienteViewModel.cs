using System;
using System.ComponentModel.DataAnnotations;

namespace SGHSSVidaPlus.MVC.Models
{
    public class HistoricoPacienteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É necessário informar o título do registro.")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "É necessário informar a descrição do histórico.")]
        public string Descricao { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataEvento { get; set; }

        public string ProfissionalResponsavel { get; set; }

        public int PacienteId { get; set; }
    }
}