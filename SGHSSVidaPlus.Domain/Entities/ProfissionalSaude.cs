using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SGHSSVidaPlus.Domain.Entities
{
    public class ProfissionalSaude
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cargo { get; set; }
        public string? EspecialidadeCargo { get; set; } // string?
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? RegistroConselho { get; set; }
        public bool Ativo { get; set; }

        public string? UsuarioInclusao { get; set; } // string?

        public DateTime DataInclusao { get; set; }
        public List<Agendamento> AgendamentosRealizados { get; set; } = new List<Agendamento>();
        public List<FormacaoAcademicaProfissionalSaude> Formacao { get; set; } = new List<FormacaoAcademicaProfissionalSaude>();
        public List<CursosCertificacoesProfissionalSaude> Cursos { get; set; } = new List<CursosCertificacoesProfissionalSaude>();
    }

    // Classe para a Formação Acadêmica do Profissional
    public class FormacaoAcademicaProfissionalSaude
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Area { get; set; } // <<< VOCÊ PRECISA ADICIONAR ESTA LINHA AQUI!
        public string InstituicaoEnsino { get; set; }
        public string AnoConclusao { get; set; }
        public string Descricao { get; set; }

        // Foreign Key e propriedade de navegação para ProfissionalSaude
        [JsonIgnore]
        public int ProfissionalSaudeId { get; set; }
        public ProfissionalSaude ProfissionalSaude { get; set; }
    }

    // Classe para Cursos e Certificações do Profissional 
    public class CursosCertificacoesProfissionalSaude
    {
        public int Id { get; set; }
        public string Titulo { get; set; } 
        public string InstituicaoEnsino { get; set; }
        public double DuracaoHoras { get; set; } 
        public string AnoConclusao { get; set; }
        public string Descricao { get; set; } 

        // Foreign Key e propriedade de navegação para ProfissionalSaude
        [JsonIgnore]
        public ProfissionalSaude ProfissionalSaude { get; set; }
        public int ProfissionalSaudeId { get; set; }
    }
}