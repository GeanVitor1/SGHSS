using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams;
using SGHSSVidaPlus.Domain.Interfaces.Repository;
using SGHSSVidaPlus.Domain.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq; // Necessário para .Any(), .Where() etc.
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

        public async Task<OperationResult> Incluir(Paciente paciente)
        {
            var result = new OperationResult();

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

            if (result.Valido)
            {
                paciente.DataInclusao = DateTime.Now;
                paciente.UsuarioInclusao = "Sistema";

                await _pacienteRepository.Incluir(paciente);
                result.Mensagens.Add("Paciente incluído com sucesso.");
            }

            return result;
        }

        public async Task<OperationResult> Editar(Paciente paciente)
        {
            var result = new OperationResult();

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

            if (result.Valido)
            {
                // Busca o paciente existente com as listas incluídas para comparação
                // Isso é essencial para que o EF Core rastreie as mudanças nas coleções
                var pacienteExistente = (await _pacienteRepository.BuscarPacientes(new PacienteParams { Id = paciente.Id, IncluirContatosHistorico = true })).FirstOrDefault();
                if (pacienteExistente == null)
                {
                    result.Valido = false;
                    result.Mensagens.Add("Paciente não encontrado para edição.");
                    return result;
                }

                // --- ATUALIZA PROPRIEDADES DE NÍVEL SUPERIOR ---
                pacienteExistente.Nome = paciente.Nome;
                pacienteExistente.DataNascimento = paciente.DataNascimento;
                pacienteExistente.Endereco = paciente.Endereco;
                pacienteExistente.CPF = paciente.CPF;
                pacienteExistente.EstadoCivil = paciente.EstadoCivil;
                pacienteExistente.Ativo = paciente.Ativo;

                // --- LÓGICA PARA ATUALIZAR LISTAS ANINHADAS (CONTATOS) ---
                // Itens a remover: Estão no DB (pacienteExistente.Contatos), mas não na lista atual da UI (paciente.Contatos)
                var contatosParaRemover = pacienteExistente.Contatos
                    .Where(cDb => !paciente.Contatos.Any(cUi => cUi.Contato == cDb.Contato && cUi.Tipo == cDb.Tipo))
                    .ToList();
                foreach (var c in contatosParaRemover)
                {
                    pacienteExistente.Contatos.Remove(c); // Remove do objeto rastreado
                }

                // Itens a adicionar: Estão na lista atual da UI (paciente.Contatos), mas não no DB
                var contatosParaAdicionar = paciente.Contatos
                    .Where(cUi => !pacienteExistente.Contatos.Any(cDb => cDb.Contato == cUi.Contato && cDb.Tipo == cUi.Tipo))
                    .ToList();
                foreach (var c in contatosParaAdicionar)
                {
                    c.PacienteId = pacienteExistente.Id; // Garante FK
                    pacienteExistente.Contatos.Add(c); // Adiciona ao objeto rastreado
                }

                // --- LÓGICA PARA ATUALIZAR LISTAS ANINHADAS (HISTÓRICO) ---
                // Itens a remover: Estão no DB (pacienteExistente.Historico), mas não na lista atual da UI (paciente.Historico)
                var historicosParaRemover = pacienteExistente.Historico
                    .Where(hDb => !paciente.Historico.Any(hUi => hUi.Titulo == hDb.Titulo && hUi.DataEvento == hDb.DataEvento))
                    .ToList();
                foreach (var h in historicosParaRemover)
                {
                    pacienteExistente.Historico.Remove(h);
                }

                // Itens a adicionar: Estão na lista atual da UI (paciente.Historico), mas não no DB
                var historicosParaAdicionar = paciente.Historico
                    .Where(hUi => !pacienteExistente.Historico.Any(hDb => hDb.Titulo == hUi.Titulo && hUi.DataEvento == hUi.DataEvento))
                    .ToList();
                foreach (var h in historicosParaAdicionar)
                {
                    h.PacienteId = pacienteExistente.Id; // Garante FK
                    pacienteExistente.Historico.Add(h);
                }

                // Salva todas as alterações no objeto pacienteExistente (que o EF Core está rastreando)
                await _pacienteRepository.Alterar(pacienteExistente); // Isso dispara o SaveChangesAsync

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
    }
}