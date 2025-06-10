using System.Collections.Generic;

namespace Selecao.MVC.Models
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
                //Permissões para candidatos
                new ClaimViewModel("candidato", "visualizar", false),
                new ClaimViewModel("candidato", "incluir", false),
                new ClaimViewModel("candidato", "alterar", false),
                new ClaimViewModel("candidato", "excluir", false),

                //Etapas Seleção
                new ClaimViewModel("etapas_selecao", "visualizar", false),
                new ClaimViewModel("etapas_selecao", "incluir", false),
                new ClaimViewModel("etapas_selecao", "alterar", false),
                new ClaimViewModel("etapas_selecao", "excluir", false),

                //Seleção
                new ClaimViewModel("selecao", "visualizar", false),
                new ClaimViewModel("selecao", "incluir", false),
                new ClaimViewModel("selecao", "alterar", false),
                new ClaimViewModel("selecao", "excluir", false),
                new ClaimViewModel("selecao", "avaliar", false),
                new ClaimViewModel("selecao", "encerrar", false),


            };

            return claims;
        }
    }
}
