using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.Interfaces.Repository; // Necessário para IRepositoryBase
using System.Collections.Generic;
using SGHSSVidaPlus.Domain.ExtensionsParams;
using System.Threading.Tasks;

namespace SGHSSVidaPlus.Domain.Interfaces.Repository
{
    // SGHSSVidaPlus.Domain.Interfaces.Repository/IProfissionalSaudeRepository.cs
    public interface IProfissionalSaudeRepository : IRepositoryBase<ProfissionalSaude>
    {
        Task<List<ProfissionalSaude>> BuscarProfissional(ProfissionalSaudeParams parametros);
        // Novo método para obter por ID com as coleções incluídas
        Task<ProfissionalSaude> ObterProfissionalComColecoes(int id);
    }
}