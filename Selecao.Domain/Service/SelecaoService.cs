using Selecao.Domain.Entities;
using Selecao.Domain.Extensions_Params;
using Selecao.Domain.Interfaces.Repository;
using Selecao.Domain.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selecao.Domain.Service
{
    public class SelecaoService : ISelecaoService
    {
        private readonly ISelecaoRepository _selecaoRepository;

        public SelecaoService(ISelecaoRepository selecaoRepository)
        {
            _selecaoRepository = selecaoRepository;
        }

        public async Task<OperationResult> Incluir(SelecaoCandidato selecao)
        {
            var validacao = new OperationResult();

            if (string.IsNullOrWhiteSpace(selecao.Cargo))
            {
                validacao.Valido = false;
                validacao.Mensagens.Add("O cargo precisa ser informado");
            }

            if(selecao.Vagas < 1)
            {
                validacao.Valido = false;
                validacao.Mensagens.Add("É preciso informar uma quantidade de vagas maior que 0");
            }

            if (string.IsNullOrWhiteSpace(selecao.UsuarioInclusao))
            {
                validacao.Valido = false;
                validacao.Mensagens.Add("é preciso informar o usuario que está incluindo a seleção");
            }

            await _selecaoRepository.Incluir(selecao);

            return validacao;
        }

        public async Task<OperationResult> IncluirCandidatos(List<CandidatoSelecaoCandidato> candidatos, int selecaoid)
        {
            var validacao = new OperationResult();

            await _selecaoRepository.IncluirCandidatos(candidatos, selecaoid);

            return validacao;
        }
    }
}
