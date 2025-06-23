
using System;
// Não usaremos outros "usings" para manter a simplicidade

namespace SGHSSVidaPlus.Domain.ExtensionsParams
{
    public class PacienteParams
    {
        public int Id { get; set; } = 0;
        public string Nome { get; set; } = string.Empty;

        // As propriedades de data de nascimento devem estar aqui
        public DateTime DataNascimentoInicio { get; set; }
        public DateTime DataNascimentoFim { get; set; }

        public string CPF { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string EstadoCivil { get; set; } = string.Empty;

        // A propriedade Ativo DEVE ser bool? (anulável)
        public bool? Ativo { get; set; }

        public string UsuarioInclusao { get; set; } = string.Empty;
        public DateTime DataInclusaoInicio { get; set; }
        public DateTime DataInclusaoFim { get; set; }

        // A propriedade StatusAtivoString foi usada na Index.cshtml para o dropdown, então a mantemos.
        public string StatusAtivoString { get; set; } = string.Empty; // Usamos este para o filtro "Ativo"/"Inativo" string

        public int TotalRegistros { get; set; } = 10;

        // Propriedade para incluir coleções na busca (usada em PacienteRepository)
        public bool IncluirContatosHistorico { get; set; }
    }
}