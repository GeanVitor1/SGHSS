using SGHSSVidaPlus.Domain.Entities;
using System;
using System.Collections.Generic;

namespace SGHSSVidaPlus.Domain.ExtensionsParams

{
    public class AgendamentoParams
    {
        public int Id { get; set; } = 0;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataHoraAgendamentoInicio { get; set; } // Para filtro por intervalo de data/hora do agendamento
        public DateTime DataHoraAgendamentoFim { get; set; }
        public bool Encerrado { get; set; } // Para filtrar por agendamentos encerrados/não encerrados
        public string UsuarioInclusao { get; set; } = string.Empty;
        public DateTime DataInclusaoInicio { get; set; }
        public DateTime DataInclusaoFim { get; set; }
        public string UsuarioEncerramento { get; set; } = string.Empty;
        public DateTime? DataEncerramentoInicio { get; set; } // Para filtro por intervalo de data de encerramento
        public DateTime? DataEncerramentoFim { get; set; }

        public bool IncluirPacientesTiposAtendimento { get; set; }
        public string StatusAgendamento { get; set; } = string.Empty;
        public string Observacoes { get; set; }

        // Adicionando IDs de relacionamento para filtro, se necessário, como no seu SelecaoParams original
        public int ProfissionalResponsavelId { get; set; } = 0;
        public int PacienteId { get; set; } = 0; // Para filtrar agendamentos de um paciente específico

        public int TotalRegistros { get; set; } = 10;
    }

}
