using Selecao.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Selecao.MVC.Models
{
    public class CandidatoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É Necessário informar o nome do candidato")]
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Logradouro { get; set; }
        public string Bairro { get; set; }
        public string EstadoCivil { get; set; }
        public bool Selecionado { get; set; }
        public bool Banido { get; set; }
        public string UsuarioInclusao { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataInclusao { get; set; }
        public List<CandidatoContato> Contatos { get; set; } = new List<CandidatoContato>();
        public List<CandidatoExperiencia> Experiencias { get; set; } = new List<CandidatoExperiencia> { };
        public List<CandidatoFormacao> Formacao { get; set; } = new List<CandidatoFormacao> { };
        public List<CandidatoCurso> Cursos { get; set; } = new List<CandidatoCurso> { };


    }
}
