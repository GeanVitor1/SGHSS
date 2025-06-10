namespace Selecao.Domain.Entities
{
    public class SelecaoCandidato
    {
        public int Id { get; set; }
        public string Cargo { get; set; }
        public int Vagas { get; set; }
        public string Objetivo { get; set; }
        public string UsuarioInclusao { get; set; }
        public bool Encerrado { get; set; }
        public DateTime DataInclusao { get; set; }
        public string UsuarioEncerramento { get; set; } = string.Empty;
        public DateTime? DataEncerramento { get; set; }
        public List<CandidatoSelecaoCandidato> Candidatos { get; set; }
        public List<EtapaSelecaoCandidato> Etapas { get; set; }
    }
}
