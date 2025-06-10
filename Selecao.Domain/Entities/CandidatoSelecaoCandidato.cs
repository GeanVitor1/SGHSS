namespace Selecao.Domain.Entities
{
    public class CandidatoSelecaoCandidato
    {
        public int Id { get; set; }
        public Candidato Candidato { get; set; }
        public int CandidatoId { get; set; }
        public SelecaoCandidato Selecao { get; set; }
        public int SelecaoId { get; set; }
        public bool Selecionado { get; set; }
        public bool Eliminado { get; set; }
    }
}
