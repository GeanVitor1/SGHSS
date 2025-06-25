using System;

namespace SGHSSVidaPlus.Domain.ExtensionsParams // <-- ESTE NAMESPACE DEVE ESTAR SEM O UNDERSCORE
{
    public class PacienteParams
    {
        public int Id { get; set; } = 0;
        public string Nome { get; set; } = string.Empty;

        public DateTime DataNascimentoInicio { get; set; }
        public DateTime DataNascimentoFim { get; set; }

        public string CPF { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string EstadoCivil { get; set; } = string.Empty;

        public bool? Ativo { get; set; }

        public string UsuarioInclusao { get; set; } = string.Empty;
        public DateTime DataInclusaoInicio { get; set; }
        public DateTime DataInclusaoFim { get; set; }

        public string StatusAtivoString { get; set; } = string.Empty;

        public int TotalRegistros { get; set; } = 10;

        public bool IncluirContatosHistorico { get; set; }
    }
}