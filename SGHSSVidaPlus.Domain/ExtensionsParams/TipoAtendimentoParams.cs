using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHSSVidaPlus.Domain.ExtensionsParams
{
    public class TipoAtendimentoParams
    {
        public int Id { get; set; } = 0;
        public string Nome { get; set; } = string.Empty;
        public bool Ativo { get; set; } = false; // Para filtrar por tipos de atendimento ativos/inativos
        public string UsuarioInclusao { get; set; } = string.Empty;
        public DateTime DataInclusaoInicio { get; set; }
        public DateTime DataInclusaoFim { get; set; }
        public int TotalRegistros { get; set; } = 10;
        public string StatusAtivoString { get; set; } = string.Empty;
    }
}
