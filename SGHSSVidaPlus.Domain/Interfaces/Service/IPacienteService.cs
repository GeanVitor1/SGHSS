    using SGHSSVidaPlus.Domain.Entities;
    using SGHSSVidaPlus.Domain.ExtensionsParams;
    using System.Collections.Generic; // Necessário para IEnumerable
    using System.Threading.Tasks;

    namespace SGHSSVidaPlus.Domain.Interfaces.Service
    {
        public interface IPacienteService
        {
            Task<OperationResult> Incluir(Paciente paciente);
            Task<OperationResult> Editar(Paciente paciente);
            Task<OperationResult> AlterarStatus(Paciente paciente);
            Task<IEnumerable<Paciente>> BuscarPacientes(PacienteParams parametros); // NOVO
        }
    }