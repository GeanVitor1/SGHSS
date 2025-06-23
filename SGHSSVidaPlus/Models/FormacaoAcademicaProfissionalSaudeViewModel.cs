using System.ComponentModel.DataAnnotations;

namespace SGHSSVidaPlus.MVC.Models
{
    public class FormacaoAcademicaProfissionalSaudeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É Necessário informar o título da formação.")]
        public string Titulo { get; set; }
        public string Area { get; set; }
        public string InstituicaoEnsino { get; set; }
        public string AnoConclusao { get; set; }
        public string Descricao { get; set; }
        public int ProfissionalSaudeId { get; set; }
    }
}