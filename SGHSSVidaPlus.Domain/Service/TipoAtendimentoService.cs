using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams;
using SGHSSVidaPlus.Domain.Interfaces.Repository;
using SGHSSVidaPlus.Domain.Interfaces.Service;
using System; // Para DateTime
using System.Threading.Tasks;

namespace SGHSSVidaPlus.Application.Services
{
    public class TipoAtendimentoService : ITipoAtendimentoService
    {
        private readonly ITipoAtendimentoRepository _tipoAtendimentoRepository;

        public TipoAtendimentoService(ITipoAtendimentoRepository tipoAtendimentoRepository)
        {
            _tipoAtendimentoRepository = tipoAtendimentoRepository;
        }

        public async Task<OperationResult> Incluir(TipoAtendimento tipoAtendimento)
        {
            var result = new OperationResult();

            if (string.IsNullOrWhiteSpace(tipoAtendimento.Nome))
            {
                result.Valido = false;
                result.Mensagens.Add("O nome do tipo de atendimento é obrigatório.");
            }

            if (result.Valido)
            {
                tipoAtendimento.DataInclusao = DateTime.Now;
                tipoAtendimento.UsuarioInclusao = "Sistema"; // Substituir pelo usuário real
                tipoAtendimento.Ativo = true; // Por padrão, ao incluir, já vem ativo

                await _tipoAtendimentoRepository.Incluir(tipoAtendimento);
                result.Mensagens.Add("Tipo de atendimento incluído com sucesso.");
            }

            return result;
        }

        public async Task<OperationResult> Editar(TipoAtendimento tipoAtendimento)
        {
            var result = new OperationResult();

            if (tipoAtendimento.Id <= 0)
            {
                result.Valido = false;
                result.Mensagens.Add("ID do tipo de atendimento inválido para edição.");
            }
            if (string.IsNullOrWhiteSpace(tipoAtendimento.Nome))
            {
                result.Valido = false;
                result.Mensagens.Add("O nome do tipo de atendimento é obrigatório para edição.");
            }

            if (result.Valido)
            {
                var tipoExistente = await _tipoAtendimentoRepository.ObterPorId(tipoAtendimento.Id);
                if (tipoExistente == null)
                {
                    result.Valido = false;
                    result.Mensagens.Add("Tipo de atendimento não encontrado para edição.");
                    return result;
                }

                tipoExistente.Nome = tipoAtendimento.Nome;
                // Não alterar UsuarioInclusao/DataInclusao aqui

                await _tipoAtendimentoRepository.Alterar(tipoExistente);
                result.Mensagens.Add("Tipo de atendimento editado com sucesso.");
            }

            return result;
        }

        public async Task<OperationResult> AlterarStatus(TipoAtendimento tipoAtendimento)
        {
            var result = new OperationResult();

            if (tipoAtendimento.Id <= 0)
            {
                result.Valido = false;
                result.Mensagens.Add("ID do tipo de atendimento inválido para alteração de status.");
            }

            if (result.Valido)
            {
                var tipoExistente = await _tipoAtendimentoRepository.ObterPorId(tipoAtendimento.Id);
                if (tipoExistente == null)
                {
                    result.Valido = false;
                    result.Mensagens.Add("Tipo de atendimento não encontrado para alterar status.");
                    return result;
                }

                tipoExistente.Ativo = tipoAtendimento.Ativo;
                await _tipoAtendimentoRepository.Alterar(tipoExistente);
                result.Mensagens.Add($"Status do tipo de atendimento alterado para {(tipoAtendimento.Ativo ? "Ativo" : "Inativo")} com sucesso.");
            }

            return result;
        }
    }
}