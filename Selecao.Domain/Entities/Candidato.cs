using System.Text.Json.Serialization;

namespace Selecao.Domain.Entities
{
    public class Candidato
    {
        public static readonly string EstadoCivilSolteiro = "SOLTEIRO";
        public static readonly string EstadoCivilCasado = "CASADO";
        public static readonly string EstadoCivilDivorciado = "DIVORCIADO";
        public static readonly string EstadoCivilViuvo = "VIUVO";

        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Logradouro { get; set; }
        public string Bairro { get; set; }
        public string EstadoCivil { get; set; }
        public bool Selecionado { get; set; }
        public bool Banido { get; set; }
        public string UsuarioInclusao { get; set; }
        public DateTime DataInclusao { get; set; }
        public List<CandidatoContato> Contatos { get; set; }
        public List<CandidatoExperiencia> Experiencias { get; set; }
        public List<CandidatoFormacao> Formacao { get; set; }
        public List<CandidatoCurso> Cursos { get; set; }
        public List<CandidatoSelecaoCandidato> Selecoes { get; set; }

        public static IEnumerable<string> GetEstadoCivil()
        {
            return typeof(Candidato)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(f => f.FieldType == typeof(string))
                .Select(f => (string)f.GetValue(null));
        }
    }

    public class CandidatoContato
    {
        public static readonly string TelefoneFixo = "Telefone Fixo";
        public static readonly string TelefoneCelular = "Telefone Celular";
        public static readonly string Email = "Email";
        public static readonly string TelefoneTerceiro = "Telefone de Terceiro";
        public int Id { get; set; }
        public string Contato { get; set; }
        public string Tipo { get; set; }
        public bool IsWhatsApp { get; set; }
        public Candidato Candidato { get; set; }
        public int CandidatoId { get; set; }

        public static IEnumerable<string> GetTiposContato()
        {
            return typeof(CandidatoContato)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(f => f.FieldType == typeof(string))
                .Select(f => (string)f.GetValue(null));
        }
    }

    public class CandidatoExperiencia
    {
        public int Id { get; set; }
        public string Cargo { get; set; }
        public string Empregador { get; set; }
        public DateTime? Inicio { get; set; }
        public DateTime? Termino { get; set; }
        public bool TrabalhoAtual { get; set; }
        public string Duracao{get;set;}
        public string Descricao { get; set; }
        public bool IsInformatica { get; set; }

        [JsonIgnore]
        public Candidato Candidato { get; set; }
        public int CandidatoId { get; set; }
    }


    public class CandidatoFormacao
    {
        public static readonly string TituloEnsinoFundamentalIncompleto = "Ensino Fundamental Incompleto";
        public static readonly string TituloEnsinoFundamental = "Ensino Fundamental";
        public static readonly string TituloEnsinoMedioIncompleto = "Ensino Médio Incompleto";
        public static readonly string TituloEnsinoMedio = "Ensino Médio";
        public static readonly string TituloGraduacaoIncompleta = "Graduação Incompleta";
        public static readonly string TituloGraduacao = "Graduação";
        public static readonly string TituloMestrado = "Mestrado";
        public static readonly string TituloDoutorado = "Doutorado";

        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Area { get; set; }
        public string InstituicaoEnsino { get; set; }
        public string AnoConclusao { get; set; }
        public string Descricao { get; set; }

        [JsonIgnore]
        public Candidato Candidato { get; set; }
        public int CandidatoId { get; set; }

        public static IEnumerable<string> GetTituloFormacao()
        {
            return typeof(CandidatoFormacao)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(f => f.FieldType == typeof(string))
                .Select(f => (string)f.GetValue(null));
        }
    }

    public class CandidatoCurso
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string InstituicaoEnsino { get; set; }
        public double DuracaoHoras { get; set; }
        public string AnoConclusao { get; set; }
        public string Descricao { get; set; }
        public string Area { get; set; }

        [JsonIgnore]
        public Candidato Candidato { get; set; }
        public int CandidatoId { get; set; }
    }
}
