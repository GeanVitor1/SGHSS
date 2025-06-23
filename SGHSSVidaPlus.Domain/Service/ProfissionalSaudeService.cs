using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams;
using SGHSSVidaPlus.Domain.Interfaces.Repository;
using SGHSSVidaPlus.Domain.Interfaces.Service;
using System; // Para DateTime
using System.Threading.Tasks;

namespace SGHSSVidaPlus.Application.Services
{
    public class ProfissionalSaudeService : IProfissionalSaudeService
    {
        private readonly IProfissionalSaudeRepository _profissionalSaudeRepository;

        public ProfissionalSaudeService(IProfissionalSaudeRepository profissionalSaudeRepository)
        {
            _profissionalSaudeRepository = profissionalSaudeRepository;
        }

        public async Task<OperationResult> Incluir(ProfissionalSaude profissionalSaude)
        {
            var result = new OperationResult();

            if (string.IsNullOrWhiteSpace(profissionalSaude.Nome))
            {
                result.Valido = false;
                result.Mensagens.Add("O nome do profissional de saúde é obrigatório.");
            }
            if (string.IsNullOrWhiteSpace(profissionalSaude.Cargo))
            {
                result.Valido = false;
                result.Mensagens.Add("O cargo do profissional de saúde é obrigatório.");
            }
            // Adicione validações para Telefone, Email, RegistroConselho se forem obrigatórios

            if (result.Valido)
            {
                profissionalSaude.DataInclusao = DateTime.Now;
                profissionalSaude.UsuarioInclusao = "Sistema"; // Substituir pelo usuário real
                profissionalSaude.Ativo = true; // Por padrão, profissional incluído já é ativo

                await _profissionalSaudeRepository.Incluir(profissionalSaude);
                result.Mensagens.Add("Profissional de saúde incluído com sucesso.");
            }

            return result;
        }

        public async Task<OperationResult> Editar(ProfissionalSaude profissionalSaude)
        {
            var result = new OperationResult();

            if (profissionalSaude.Id <= 0)
            {
                result.Valido = false;
                result.Mensagens.Add("ID do profissional de saúde inválido para edição.");
            }
            if (string.IsNullOrWhiteSpace(profissionalSaude.Nome))
            {
                result.Valido = false;
                result.Mensagens.Add("O nome do profissional de saúde é obrigatório para edição.");
            }

            if (result.Valido)
            {
                var profissionalExistente = await _profissionalSaudeRepository.ObterPorId(profissionalSaude.Id);
                if (profissionalExistente == null)
                {
                    result.Valido = false;
                    result.Mensagens.Add("Profissional de saúde não encontrado para edição.");
                    return result;
                }

                profissionalExistente.Nome = profissionalSaude.Nome;
                profissionalExistente.Cargo = profissionalSaude.Cargo;
                profissionalExistente.Telefone = profissionalSaude.Telefone;
                profissionalExistente.Email = profissionalSaude.Email;
                // Formacao e Cursos podem ser mais complexos para editar aqui, dependendo da regra de negócio
                // Por simplicidade, estamos apenas atualizando os campos diretos.

                await _profissionalSaudeRepository.Alterar(profissionalExistente);
                result.Mensagens.Add("Profissional de saúde editado com sucesso.");
            }

            return result;
        }

        public async Task<OperationResult> AlterarStatus(ProfissionalSaude profissionalSaude)
        {
            var result = new OperationResult();

            if (profissionalSaude.Id <= 0)
            {
                result.Valido = false;
                result.Mensagens.Add("ID do profissional de saúde inválido para alteração de status.");
            }

            if (result.Valido)
            {
                var profissionalExistente = await _profissionalSaudeRepository.ObterPorId(profissionalSaude.Id);
                if (profissionalExistente == null)
                {
                    result.Valido = false;
                    result.Mensagens.Add("Profissional de saúde não encontrado para alterar status.");
                    return result;
                }

                profissionalExistente.Ativo = profissionalSaude.Ativo;
                await _profissionalSaudeRepository.Alterar(profissionalExistente);
                result.Mensagens.Add($"Status do profissional de saúde alterado para {(profissionalSaude.Ativo ? "Ativo" : "Inativo")} com sucesso.");
            }

            return result;
        }
    }
}