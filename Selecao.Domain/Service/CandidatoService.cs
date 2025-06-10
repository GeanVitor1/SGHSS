using Selecao.Domain.Entities;
using Selecao.Domain.Extensions_Params;
using Selecao.Domain.Interfaces.Repository;
using Selecao.Domain.Interfaces.Service;

namespace Selecao.Domain.Service
{
    public class CandidatosService : ICandidatosService
    {
        private readonly ICandidatosRepository _candidatoRepository;

        public CandidatosService(ICandidatosRepository candidatoRepository)
        {
            _candidatoRepository = candidatoRepository;
        }

        public async Task<OperationResult> Incluir(Candidato candidato)
        {
            var validacao = new OperationResult();
            if (string.IsNullOrWhiteSpace(candidato.Nome))
            {
                validacao.Valido = false;
                validacao.Mensagens.Add("Não é possivel adicionar um candidato sem um nome");
            }

            var candidatoBd = await _candidatoRepository.BuscarCandidatos(new CandidatoParams()
            {
                Nome = candidato.Nome.Trim()
            });
            if (candidatoBd.Any())
            {
                validacao.Valido = false;
                validacao.Mensagens.Add("Não é possivel adicionar um candidato já cadastrado");
            }

            if (!validacao.Valido)
                return validacao;

            await _candidatoRepository.Incluir(candidato);

            return validacao;
        }
        public async Task<OperationResult> Editar(Candidato candidato)
        {
            var validacao = new OperationResult();

            if (string.IsNullOrWhiteSpace(candidato.Nome))
            {
                validacao.Valido = false;
                validacao.Mensagens.Add("Não é possivel adicionar um candidato sem um nome");
            }

            var candidatoBd = await _candidatoRepository.BuscarCandidatos(new CandidatoParams()
            {
                Nome = candidato.Nome.Trim()
            });

            if (candidatoBd.Any(c => c.Nome == candidato.Nome && c.Id != candidato.Id))
            {
                validacao.Valido = false;
                validacao.Mensagens.Add("Já existe um candidato com esse nome");
            }

            if (!validacao.Valido)
                return validacao;

            await _candidatoRepository.Editar(candidato);

            return validacao;
        }

        public async Task<OperationResult> AlterarStatus(Candidato candidato)
        {
            var validacao = new OperationResult();

            candidato.Banido = !candidato.Banido;

            await _candidatoRepository.Alterar(candidato);

            return validacao;
        }
    }
}
