using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.Interfaces.Repository; // Necessário para IRepositoryBase
using System.Collections.Generic;
using SGHSSVidaPlus.Domain.ExtensionsParams;
using System.Threading.Tasks;

namespace SGHSSVidaPlus.Domain.Interfaces.Repository
{
    public interface IProfissionalSaudeRepository : IRepositoryBase<ProfissionalSaude>
    {
        Task<List<ProfissionalSaude>> BuscarProfissionais(ProfissionalSaudeParams parametros);
    }
}