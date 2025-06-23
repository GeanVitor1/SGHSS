using SGHSSVidaPlus.Domain.Entities;
using SGHSSVidaPlus.Domain.ExtensionsParams;
using SGHSSVidaPlus.Domain.Interfaces.Repository; // Necessário para IRepositoryBase
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGHSSVidaPlus.Domain.Interfaces.Repository
{
    public interface IPacienteRepository : IRepositoryBase<Paciente>
    {
        Task<List<Paciente>> BuscarPacientes(PacienteParams parametros);
    }
}