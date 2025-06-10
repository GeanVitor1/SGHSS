namespace Selecao.Domain.Entities
{
    public class CandidatoParams
    {
        public int Id { get; set; } = 0;
        public string Nome { get; set; } = string.Empty;
        public DateTime DataNascimentoInicio { get; set; }
        public DateTime DataNascimentoFim { get; set; }
        public string Endereço { get; set; } = string.Empty;
        public string EstadoCivil { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool Selecionado { get; set; }
        public bool Banido { get; set; }
        public string UsuarioInclusao { get; set; } = string.Empty;
        public DateTime DataInclusaoInicio { get; set; }
        public DateTime DataInclusaoFim { get; set; }
        public List<CandidatoContato> Contatos { get; set; } = new List<CandidatoContato>();
        public int TotalRegistros { get; set; } = 10;
        public bool InfoExtras { get; set; }
    }
}
