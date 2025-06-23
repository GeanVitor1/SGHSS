using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SGHSSVidaPlus.Domain.Entities
{

    public class TipoAtendimento
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; } = true; 
        public string UsuarioInclusao { get; set; }
        public DateTime DataInclusao { get; set; }

        public List<AgendamentoTipoAtendimento> AgendamentosAssociados { get; set; }
    }

    public class AgendamentoTipoAtendimento
    {
        public int Id { get; set; }


        public TipoAtendimento TipoAtendimento { get; set; }
        public int TipoAtendimentoId { get; set; }

        public Agendamento Agendamento { get; set; }
        public int AgendamentoId { get; set; }


    }
}