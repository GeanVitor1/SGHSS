using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHSSVidaPlus.Domain.ExtensionsParams
{
    public class ProfissionalSaudeParams
{
    public int Id { get; set; } = 0;
    public string Nome { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty; // Usando "Cargo" como na sua entidade
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Ativo { get; set; } // Para filtrar por profissionais ativos/inativos
    public string UsuarioInclusao { get; set; } = string.Empty;
    public DateTime DataInclusaoInicio { get; set; }
    public DateTime DataInclusaoFim { get; set; }
    public int TotalRegistros { get; set; } = 10;

    public bool IncluirFormacaoCursos { get; set; }
    }
}
