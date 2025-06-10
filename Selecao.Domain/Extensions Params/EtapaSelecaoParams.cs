using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selecao.Domain.Extensions_Params
{
    public class EtapaSelecaoParams
    {
        public int Id { get; set; } = 0;
        public string Nome { get; set; } = string.Empty;
        public bool Status { get; set; } = false;
        public string UsuarioInclusao { get; set; } = string.Empty;
        public DateTime DataInclusaoInicio { get; set; }
        public DateTime DataInclusaoFim { get; set; }
        public string StatusString { get; set; } = string.Empty;
        public int TotalRegistros { get; set; } = 10;
    }
}
