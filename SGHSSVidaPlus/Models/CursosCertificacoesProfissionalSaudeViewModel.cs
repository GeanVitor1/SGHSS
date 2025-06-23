using System.ComponentModel.DataAnnotations;

namespace SGHSSVidaPlus.MVC.Models
{
    public class CursosCertificacoesProfissionalSaudeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É Necessário informar o título do curso.")]
        public string Titulo { get; set; }
        public string InstituicaoEnsino { get; set; }

        [Required(ErrorMessage = "É Necessário informar a duração em horas.")]
        public double DuracaoHoras { get; set; }
        public string AnoConclusao { get; set; }
        public string Descricao { get; set; }
        public string Area { get; set; }
        public int ProfissionalSaudeId { get; set; }
    }
}