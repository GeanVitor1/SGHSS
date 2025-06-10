using Selecao.Domain.Entities;

namespace Selecao.MVC.Models
{
    public class SelecaoCandidatoViewModel
    {
        public int Id { get; set; }
        public string Cargo { get; set; }
        public int Vagas { get; set; }
        public string Objetivo { get; set; }
        public List<CandidatoSelectViewModel> Candidatos { get; set; }
        public List<EtapaSelectViewModel> Etapas { get; set; }
    }

    public class CandidatoSelectViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Logradouro { get; set; }
        public string Bairro { get; set; }
        public string EstadoCivil { get; set; }
        public bool Selecionado { get; set; }
        public bool Banido { get; set; }
        public string UsuarioInclusao { get; set; }
        public DateTime DataInclusao { get; set; }
        public List<CandidatoContato> Contatos { get; set; }
        public List<CandidatoExperiencia> Experiencias { get; set; }
        public List<CandidatoFormacao> Formacao { get; set; }
        public List<CandidatoCurso> Cursos { get; set; }
        public bool IsSelected { get; set; }
    }
    public class EtapaSelectViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool IsSelected { get; set; }
    }
}
