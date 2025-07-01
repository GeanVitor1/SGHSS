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
            // ... outras validações para propriedades escalares ...

            if (result.Valido)
            {
                // 1. Obter o profissional existente do banco de dados COM suas coleções
                var profissionalExistente = await _profissionalSaudeRepository.ObterProfissionalComColecoes(profissionalSaude.Id);

                if (profissionalExistente == null)
                {
                    result.Valido = false;
                    result.Mensagens.Add("Profissional de saúde não encontrado para edição.");
                    return result;
                }

                // 2. Atualizar propriedades escalares
                profissionalExistente.Nome = profissionalSaude.Nome;
                profissionalExistente.Cargo = profissionalSaude.Cargo;
                profissionalExistente.Telefone = profissionalSaude.Telefone;
                profissionalExistente.Email = profissionalSaude.Email;
                profissionalExistente.RegistroConselho = profissionalSaude.RegistroConselho; // Se aplicável
                                                                                             // Não altere UsuarioInclusao ou DataInclusao aqui, a menos que seja intencional.
                                                                                             // profissionalExistente.Ativo = profissionalSaude.Ativo; // Se a alteração de status for feita por outro método ou campo.

                // 3. Sincronizar Formações Acadêmicas
                // Remover as formações que não estão mais na lista enviada
                foreach (var formacaoExistente in profissionalExistente.Formacao.ToList()) // .ToList() para evitar modificação durante iteração
                {
                    if (!profissionalSaude.Formacao.Any(nf =>
                        nf.Titulo == formacaoExistente.Titulo &&
                        nf.InstituicaoEnsino == formacaoExistente.InstituicaoEnsino)) // Assumindo Título e Instituição como chaves para identificar uma formação
                    {
                        profissionalExistente.Formacao.Remove(formacaoExistente);
                    }
                }

                // Adicionar ou atualizar formações
                foreach (var novaFormacao in profissionalSaude.Formacao)
                {
                    var formacaoExistente = profissionalExistente.Formacao.FirstOrDefault(f =>
                        f.Titulo == novaFormacao.Titulo &&
                        f.InstituicaoEnsino == novaFormacao.InstituicaoEnsino);

                    if (formacaoExistente == null)
                    {
                        // Nova formação, adicione
                        profissionalExistente.Formacao.Add(novaFormacao);
                    }
                    else
                    {
                        // Formação existente, atualize as propriedades que podem mudar
                        formacaoExistente.Area = novaFormacao.Area;
                        formacaoExistente.AnoConclusao = novaFormacao.AnoConclusao;
                        // ... outras propriedades da formação ...
                    }
                }

                // 4. Sincronizar Cursos e Certificações (lógica similar)
                foreach (var cursoExistente in profissionalExistente.Cursos.ToList())
                {
                    if (!profissionalSaude.Cursos.Any(nc =>
                        nc.Titulo == cursoExistente.Titulo &&
                        nc.InstituicaoEnsino == cursoExistente.InstituicaoEnsino &&
                        nc.DuracaoHoras == cursoExistente.DuracaoHoras)) // Assumindo estes como chaves para identificar um curso
                    {
                        profissionalExistente.Cursos.Remove(cursoExistente);
                    }
                }

                foreach (var novoCurso in profissionalSaude.Cursos)
                {
                    var cursoExistente = profissionalExistente.Cursos.FirstOrDefault(c =>
                        c.Titulo == novoCurso.Titulo &&
                        c.InstituicaoEnsino == novoCurso.InstituicaoEnsino &&
                        c.DuracaoHoras == novoCurso.DuracaoHoras);

                    if (cursoExistente == null)
                    {
                        // Novo curso, adicione
                        profissionalExistente.Cursos.Add(novoCurso);
                    }
                    else
                    {
                        // Curso existente, atualize as propriedades que podem mudar
                        // (Seus cursos têm DuracaoHoras, então verifique se há outras propriedades a atualizar)
                        // cursoExistente.AlgumaOutraPropriedade = novoCurso.AlgumaOutraPropriedade;
                    }
                }

                // 5. Salvar as mudanças
                // O método Alterar do RepositoryBase deve chamar _context.SaveChanges()
                // Se o RepositoryBase.Alterar apenas faz um _context.Entry(entity).State = EntityState.Modified;,
                // ele precisará ser mais inteligente ou você precisará chamar SaveChanges no serviço
                // após manipular as coleções, se o contexto for compartilhado.
                // Dada a estrutura do seu RepositoryBase (que não foi fornecido, mas é comum),
                // a simples chamada de Alterar(profissionalExistente) após as manipulações das coleções
                // fará com que o EF rastreie as mudanças e as salve.
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