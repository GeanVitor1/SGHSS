using System; // Necessário para IDisposable
using System.Collections.Generic;
using System.Linq.Expressions; // Necessário para Expression<Func<TEntity, bool>>
using System.Threading.Tasks;
using SGHSSVidaPlus.Domain.ExtensionsParams;

namespace SGHSSVidaPlus.Domain.Interfaces.Repository
{
    public interface IRepositoryBase<TEntity> : IDisposable where TEntity : class
    {
        Task Incluir(TEntity entity);
        Task Alterar(TEntity entity);
        Task Excluir(int id);
        Task Excluir(TEntity entity);
        Task<IEnumerable<TEntity>> ObterTodos();
        Task<TEntity> ObterPorId(int id);
        Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate, int quantidadeRegistros);
    }
}