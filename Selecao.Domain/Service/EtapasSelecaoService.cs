using Selecao.Domain.Entities;
using Selecao.Domain.Extensions_Params;
using Selecao.Domain.Interfaces.Repository;
using Selecao.Domain.Interfaces.Service;

namespace Selecao.Domain.Service
{
    public class EtapasSelecaoService : IEtapasSelecaoService
    {
        private readonly IEtapaSelecaoRepository _etapaSelecaoRepository;

        public EtapasSelecaoService(IEtapaSelecaoRepository etapaSelecaoRepository)
        {
            _etapaSelecaoRepository = etapaSelecaoRepository;
        }

        public async Task<OperationResult> Incluir(EtapaSelecao etapa)
        {
            var validacao = new OperationResult();
            if (string.IsNullOrWhiteSpace(etapa.Nome))
            {
                validacao.Valido = false;
                validacao.Mensagens.Add("Não é possivel adicionar uma etapa sem um nome");
            }
            else
            {
                etapa.Nome = etapa.Nome.ToUpper();
            }

            var validacaoBd = await _etapaSelecaoRepository.Buscar(c => c.Nome == etapa.Nome);
            if (validacaoBd.Any())
            {
                validacao.Valido = false;
                validacao.Mensagens.Add("Já existe uma etapa com esse nome");
            }



            if (!validacao.Valido)
                return validacao;


            await _etapaSelecaoRepository.Incluir(etapa);

            return validacao;
        }
        public async Task<OperationResult> Editar(EtapaSelecao etapa)
        {
            var validacao = new OperationResult();

            var etapaBd = await _etapaSelecaoRepository.ObterPorId(etapa.Id);

            etapaBd.Nome = etapa.Nome;

            await _etapaSelecaoRepository.Alterar(etapaBd);

            return validacao;
        }

        public async Task<OperationResult> AlterarStatus(EtapaSelecao etapa)
        {
            var validacao = new OperationResult();

            etapa.Status = !etapa.Status;

            await _etapaSelecaoRepository.Alterar(etapa);

            return validacao;
        }
    }
}
