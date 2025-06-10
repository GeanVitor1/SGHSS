namespace Selecao.Domain.Entities
{
    public class EtapaSelecaoCandidato
    {
        public int Id { get; set; }
        public EtapaSelecao Etapa { get; set; }
        public int EtapaId { get; set; }
        public SelecaoCandidato Selecao { get; set; }
        public int SelecaoId { get; set; }
    }
}
