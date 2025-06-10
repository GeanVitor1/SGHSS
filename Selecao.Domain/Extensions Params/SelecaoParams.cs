using Selecao.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selecao.Domain.Extensions_Params
{
    public class SelecaoParams
    {
        public int Id { get; set; } = 0;
        public string Cargo { get; set; } = string.Empty;
        public int Vagas { get; set; } = 0;
        public string Objetivo { get; set; } = string.Empty;
        public string UsuarioInclusao { get; set; } = string.Empty;
        public bool Encerrado { get; set; }
        public string Status {  get; set; } = string.Empty;
        public DateTime DataInclusao { get; set; }
        public string UsuarioEncerramento { get; set; } = string.Empty;
        public DateTime? DataEncerramento { get; set; }
        public bool InfoExtras {  get; set; }
        public int TotalRegistros { get; set; } = 10;
    }
}
