using System.Linq.Expressions;

namespace Selecao.Domain.Interfaces.Repository
{
    public interface IRepositoryBase<TEntity> : IDisposable where TEntity : class
    {
        Task Incluir(TEntity entity);
        Task Alterar(TEntity entity);
        Task Excluir(Guid id);
        Task Excluir(TEntity entity);
        Task<IEnumerable<TEntity>> ObterTodos();
        Task<TEntity> ObterPorId(Guid id);
        Task<TEntity> ObterPorId(int id);
        Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate, int quantidadeRegistros);
    }
}
