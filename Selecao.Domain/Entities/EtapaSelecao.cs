namespace Selecao.Domain.Entities
{
    public class EtapaSelecao
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Status { get; set; } = true;
        public string UsuarioInclusao { get; set; }
        public DateTime DataInclusao { get; set; }
        public List<EtapaSelecaoCandidato> Selecoes { get; set; }

        //public List<Selecao> Selecao { get; set; }

    }
}
