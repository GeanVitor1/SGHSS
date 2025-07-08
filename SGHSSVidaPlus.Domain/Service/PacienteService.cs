// SGHSSVidaPlus.Application.Services/PacienteService.cs

using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams;
using SGHSSVidaPlus.Domain.Interfaces.Repository;
using SGHSSVidaPlus.Domain.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGHSSVidaPlus.Application.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly IPacienteRepository _pacienteRepository;

        public PacienteService(IPacienteRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository;
        }

        // --- Método auxiliar para limpar o CPF (Centralizado aqui) ---
        private string CleanCpf(string? cpf) // Tornar string? para lidar com nulos
        {
            return new string(cpf?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>()); // Lida com CPF nulo
        }
        // --- Fim do método auxiliar ---

        public async Task<OperationResult> Incluir(Paciente paciente)
        {
            var result = new OperationResult();

            // Limpa o CPF ANTES de qualquer validação ou uso.
            // O CPF já virá do ViewModel (RegisterInput.CPF ou PacienteAdminRegisterViewModel.CPF)
            paciente.CPF = CleanCpf(paciente.CPF);

            if (string.IsNullOrWhiteSpace(paciente.Nome))
            {
                result.Valido = false;
                result.Mensagens.Add("O nome do paciente é obrigatório.");
            }
            if (string.IsNullOrWhiteSpace(paciente.CPF))
            {
                result.Valido = false;
                result.Mensagens.Add("O CPF do paciente é obrigatório.");
            }
            else if (paciente.CPF.Length < 11 || paciente.CPF.Length > 11) // Basicamente, CPF deve ter 11 dígitos após limpeza
            {
                // Opcional: Adicionar validação de tamanho para CPF já limpo
                // result.Valido = false;
                // result.Mensagens.Add("O CPF deve conter 11 dígitos.");
            }

            // Verifica se o CPF já existe
            // A busca aqui usará o CPF JÁ LIMPO
            // Para INCLUSÃO, se um paciente com ESSE CPF JÁ EXISTE, é um duplicado.
            var pacienteComCpfExistente = (await _pacienteRepository.BuscarPacientes(new PacienteParams { CPF = paciente.CPF })).FirstOrDefault();

            // Para Inclusão: Se pacienteComCpfExistente não for null, significa que já existe.
            // A condição `pacienteComCpfExistente.Id != paciente.Id` só é relevante para o MÉTODO DE EDIÇÃO.
            if (pacienteComCpfExistente != null)
            {
                result.Valido = false;
                result.Mensagens.Add("Já existe um paciente cadastrado com este CPF.");
            }

            if (result.Valido)
            {
                if (string.IsNullOrWhiteSpace(paciente.UsuarioInclusao))
                    paciente.UsuarioInclusao = "Sistema (Auto-cadastro)";
                if (paciente.DataInclusao == DateTime.MinValue)
                    paciente.DataInclusao = DateTime.Now;

                await _pacienteRepository.Incluir(paciente);
                result.Mensagens.Add("Paciente incluído com sucesso.");
            }

            return result;
        }

        public async Task<OperationResult> Editar(Paciente paciente)
        {
            var result = new OperationResult();

            // Limpa o CPF também na edição para manter a padronização
            paciente.CPF = CleanCpf(paciente.CPF);

            if (paciente.Id <= 0)
            {
                result.Valido = false;
                result.Mensagens.Add("ID do paciente inválido para edição.");
            }
            if (string.IsNullOrWhiteSpace(paciente.Nome))
            {
                result.Valido = false;
                result.Mensagens.Add("O nome do paciente é obrigatório para edição.");
            }
            if (string.IsNullOrWhiteSpace(paciente.CPF))
            {
                result.Valido = false;
                result.Mensagens.Add("O CPF do paciente é obrigatório.");
            }

            // Verifica duplicidade de CPF ao editar
            var pacienteComCpfExistente = (await _pacienteRepository.BuscarPacientes(new PacienteParams { CPF = paciente.CPF })).FirstOrDefault();
            // Para Edição: Se pacienteComCpfExistente não for null E o ID for diferente do paciente que está sendo editado, é um duplicado.
            if (pacienteComCpfExistente != null && pacienteComCpfExistente.Id != paciente.Id)
            {
                result.Valido = false;
                result.Mensagens.Add("Já existe outro paciente cadastrado com este CPF.");
            }

            if (result.Valido)
            {
                var pacienteExistente = (await _pacienteRepository.BuscarPacientes(new PacienteParams { Id = paciente.Id, IncluirContatosHistorico = true })).FirstOrDefault();
                if (pacienteExistente == null)
                {
                    result.Valido = false;
                    result.Mensagens.Add("Paciente não encontrado para edição.");
                    return result;
                }

                // Atualiza propriedades de nível superior
                pacienteExistente.Nome = paciente.Nome;
                pacienteExistente.DataNascimento = paciente.DataNascimento;
                pacienteExistente.Endereco = paciente.Endereco;
                pacienteExistente.CPF = paciente.CPF; // CPF já vem limpo
                pacienteExistente.EstadoCivil = paciente.EstadoCivil;
                pacienteExistente.Ativo = paciente.Ativo;

                // Lógica para atualizar listas aninhadas (Contatos)
                var contatosParaRemover = pacienteExistente.Contatos
                    .Where(cDb => !paciente.Contatos.Any(cUi => cUi.Contato == cDb.Contato && cUi.Tipo == cDb.Tipo))
                    .ToList();
                foreach (var c in contatosParaRemover)
                {
                    pacienteExistente.Contatos.Remove(c);
                }

                var contatosParaAdicionar = paciente.Contatos
                    .Where(cUi => !pacienteExistente.Contatos.Any(cDb => cDb.Contato == cUi.Contato && cDb.Tipo == cUi.Tipo))
                    .ToList();
                foreach (var c in contatosParaAdicionar)
                {
                    c.PacienteId = pacienteExistente.Id;
                    pacienteExistente.Contatos.Add(c);
                }

                // Lógica para atualizar listas aninhadas (Histórico)
                var historicosParaRemover = pacienteExistente.Historico
                    .Where(hDb => !paciente.Historico.Any(hUi => hUi.Titulo == hDb.Titulo && hUi.DataEvento == hDb.DataEvento))
                    .ToList();
                foreach (var h in historicosParaRemover)
                {
                    pacienteExistente.Historico.Remove(h);
                }

                var historicosParaAdicionar = paciente.Historico
                    .Where(hUi => !pacienteExistente.Historico.Any(hDb => hDb.Titulo == hUi.Titulo && hDb.DataEvento == hUi.DataEvento))
                    .ToList();
                foreach (var h in historicosParaAdicionar)
                {
                    h.PacienteId = pacienteExistente.Id;
                    pacienteExistente.Historico.Add(h);
                }

                await _pacienteRepository.Alterar(pacienteExistente);

                result.Mensagens.Add("Paciente editado com sucesso.");
            }

            return result;
        }

        public async Task<OperationResult> AlterarStatus(Paciente paciente)
        {
            var result = new OperationResult();

            if (paciente.Id <= 0)
            {
                result.Valido = false;
                result.Mensagens.Add("ID do paciente inválido para alteração de status.");
            }

            if (result.Valido)
            {
                var pacienteExistente = await _pacienteRepository.ObterPorId(paciente.Id);
                if (pacienteExistente == null)
                {
                    result.Valido = false;
                    result.Mensagens.Add("Paciente não encontrado para alterar status.");
                    return result;
                }

                pacienteExistente.Ativo = paciente.Ativo;
                await _pacienteRepository.Alterar(pacienteExistente);
                result.Mensagens.Add($"Status do paciente alterado para {(paciente.Ativo ? "Ativo" : "Inativo")} com sucesso.");
            }

            return result;
        }

        public async Task<IEnumerable<Paciente>> BuscarPacientes(PacienteParams parametros)
        {
            // Limpa o CPF dos parâmetros de busca, se houver, antes de passar para o repositório
            parametros.CPF = CleanCpf(parametros.CPF);
            return await _pacienteRepository.BuscarPacientes(parametros);
        }
    }
}