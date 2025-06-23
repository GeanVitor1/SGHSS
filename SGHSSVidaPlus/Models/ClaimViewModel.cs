using System.Collections.Generic;
using System.Linq; // Necessário para .Distinct() e .Count()

namespace SGHSSVidaPlus.MVC.Models // Namespace atualizado
{
    public class ClaimViewModel
    {
        public string Tipo { get; set; }
        public string Valor { get; set; }
        public bool IsSelected { get; set; }

        public ClaimViewModel() { }
        public ClaimViewModel(string tipo, string valor, bool isSelected)
        {
            Tipo = tipo;
            Valor = valor;
            IsSelected = isSelected;
        }

        public static List<ClaimViewModel> ObterTodas()
        {
            var claims = new List<ClaimViewModel>
            {
                // Permissões para Pacientes
                new ClaimViewModel("paciente", "visualizar", false),
                new ClaimViewModel("paciente", "incluir", false),
                new ClaimViewModel("paciente", "alterar", false),
                new ClaimViewModel("paciente", "excluir", false),

                // Permissões para Profissionais de Saúde
                new ClaimViewModel("profissional_saude", "visualizar", false),
                new ClaimViewModel("profissional_saude", "incluir", false),
                new ClaimViewModel("profissional_saude", "alterar", false),
                new ClaimViewModel("profissional_saude", "excluir", false),

                // Permissões para Agendamentos
                new ClaimViewModel("agendamento", "visualizar", false),
                new ClaimViewModel("agendamento", "incluir", false),
                new ClaimViewModel("agendamento", "alterar", false),
                new ClaimViewModel("agendamento", "cancelar", false),
                new ClaimViewModel("agendamento", "encerrar", false),

                // Permissões para Tipos de Atendimento
                new ClaimViewModel("tipo_atendimento", "visualizar", false),
                new ClaimViewModel("tipo_atendimento", "incluir", false),
                new ClaimViewModel("tipo_atendimento", "alterar", false),
                new ClaimViewModel("tipo_atendimento", "excluir", false)
            };

            return claims;
        }
    }
}