using Microsoft.EntityFrameworkCore;
using Selecao.Domain.Interfaces.Repository;
using Selecao.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Selecao.Infrastructure.Data.Repository
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        protected DbSet<TEntity> _dbSet;
        protected readonly SelecaoContext _context;

        public RepositoryBase(SelecaoContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task Incluir(TEntity entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task Alterar(TEntity entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Excluir(Guid id)
        {
            var entity = _dbSet.Find(id);
            await Excluir(entity);
        }

        public virtual async Task Excluir(TEntity entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
                _dbSet.Attach(entity);

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> ObterTodos()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<TEntity> ObterPorId(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<TEntity> ObterPorId(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }
        public async Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate, int quantidadeRegistros)
        {
            if (quantidadeRegistros > 0)
                return await _dbSet.Where(predicate).Take(quantidadeRegistros).ToListAsync();

            return await Buscar(predicate);
        }

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
