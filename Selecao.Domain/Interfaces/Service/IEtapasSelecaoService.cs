using Selecao.Domain.Entities;
using Selecao.Domain.Extensions_Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selecao.Domain.Interfaces.Service
{
    public interface IEtapasSelecaoService
    {
        Task<OperationResult> Incluir(EtapaSelecao etapa);

        Task<OperationResult> Editar(EtapaSelecao etapa);

        Task<OperationResult> AlterarStatus(EtapaSelecao etapa);
    }
}
