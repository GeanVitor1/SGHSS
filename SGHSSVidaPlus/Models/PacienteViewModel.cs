using SGHSSVidaPlus.Domain.Entities; // Namespace da entidade atualizado
using System.ComponentModel.DataAnnotations; // Mantido para validações
using System; // Para DateTime
using System.Collections.Generic; // Para List

namespace SGHSSVidaPlus.MVC.Models // Namespace do projeto MVC atualizado
{
    public class PacienteViewModel // Nome da classe atualizado
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É Necessário informar o nome do paciente")] // Mensagem atualizada
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string CPF { get; set; } // Adicionado CPF
        public string Endereco { get; set; } // Logradouro e Bairro combinados em Endereco
        public string EstadoCivil { get; set; }
        public bool Ativo { get; set; } // Substitui Selecionado e Banido
        public string UsuarioInclusao { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataInclusao { get; set; }

        // Relacionamentos com ViewModels aninhados (baseados nas entidades auxiliares)
        public List<PacienteContatoViewModel> Contatos { get; set; } = new List<PacienteContatoViewModel>();
        public List<HistoricoPacienteViewModel> Historico { get; set; } = new List<HistoricoPacienteViewModel>(); // Substitui Experiencias

        // Removidos: Formacao e Cursos, pois agora pertencem a ProfissionalSaude
        // public List<FormacaoAcademicaProfissionalSaudeViewModel> Formacao { get; set; } = new List<FormacaoAcademicaProfissionalSaudeViewModel>();
        // public List<CursosCertificacoesProfissionalSaudeViewModel> Cursos { get; set; } = new List<CursosCertificacoesProfissionalSaudeViewModel>();
    }
}