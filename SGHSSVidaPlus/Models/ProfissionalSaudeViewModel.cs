// SGHSSVidaPlus.MVC/Models/ProfissionalSaudeViewModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SGHSSVidaPlus.MVC.Models
{
    public class ProfissionalSaudeViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "É Necessário informar o nome do profissional.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "É Necessário informar o cargo/especialidade.")]
        public string Cargo { get; set; }
        public string EspecialidadeCargo { get; set; } // Se usar como campo separado
        public string RegistroConselho { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public bool Ativo { get; set; }
        public string UsuarioInclusao { get; set; }
        public DateTime DataInclusao { get; set; }

        public List<FormacaoAcademicaProfissionalSaudeViewModel> Formacao { get; set; } = new List<FormacaoAcademicaProfissionalSaudeViewModel>();
        public List<CursosCertificacoesProfissionalSaudeViewModel> Cursos { get; set; } = new List<CursosCertificacoesProfissionalSaudeViewModel>();
    }
}