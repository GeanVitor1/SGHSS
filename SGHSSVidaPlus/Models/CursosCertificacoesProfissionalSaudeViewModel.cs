using System.ComponentModel.DataAnnotations;

namespace SGHSSVidaPlus.MVC.Models
{
    public class CursosCertificacoesProfissionalSaudeViewModel
    {
        public int? Id { get; set; } // Permitir que seja nulo em novas adições

        [Required(ErrorMessage = "É Necessário informar o título do curso.")]
        public string Titulo { get; set; }
        public string InstituicaoEnsino { get; set; }
        public string Area { get; set; }
        public double? DuracaoHoras { get; set; } // double? para permitir nulos
        public string AnoConclusao { get; set; } // Mantido como string
        public string Descricao { get; set; }

        // REMOVIDO: public int ProfissionalSaudeId { get; set; }
    }
}