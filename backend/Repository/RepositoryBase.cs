using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected RepositoryContext _RepositoryContext;

        public RepositoryBase(RepositoryContext repositoryContext)
        {
            _RepositoryContext = repositoryContext;
        }

        // track changes for read-only queries
        public IQueryable<T> FindAll(bool trackChanges) => 
            !trackChanges ? _RepositoryContext.Set<T>().AsNoTracking() : _RepositoryContext.Set<T>();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges) => 
            !trackChanges ? _RepositoryContext.Set<T>().Where(expression).AsNoTracking() : _RepositoryContext.Set<T>().Where(expression);

        public void Create(T entity) => 
            _RepositoryContext.Set<T>().Add(entity);

        public void Update(T entity) => 
            _RepositoryContext.Set<T>().Update(entity);

        public void Delete(T entity) => 
            _RepositoryContext.Set<T>().Remove(entity);
    }
}
