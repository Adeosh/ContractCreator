using ContractCreator.Domain.Interfaces;
using ContractCreator.Infrastructure.Data;
using ContractCreator.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ContractCreator.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _dbContext;

        public Repository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T?> GetByIdAsync(int id) => await _dbContext.Set<T>().FindAsync(id);

        public async Task<IEnumerable<T>> ListAllAsync() =>
            await _dbContext.Set<T>().AsNoTracking().ToListAsync();

        public async Task<IEnumerable<T>> ListAsync(ISpecification<T> spec) =>
            await ApplySpecification(spec).AsNoTracking().ToListAsync();

        public async Task<T?> FirstOrDefaultAsync(ISpecification<T> spec) =>
            await ApplySpecification(spec).FirstOrDefaultAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await _dbContext.Set<T>().AsNoTracking().Where(predicate).ToListAsync();

        public async Task<int> CountAsync(ISpecification<T> spec) =>
            await ApplySpecification(spec).CountAsync();

        public async Task AddAsync(T entity) =>
            await _dbContext.Set<T>().AddAsync(entity);

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Set<T>().Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Применяет спецификацию к набору данных (DbSet) текущего контекста.
        /// <para>
        /// Метод использует <see cref="SpecificationEvaluator{T}"/> для трансляции условий спецификации
        /// в IQueryable выражение Entity Framework.
        /// </para>
        /// </summary>
        /// <param name="spec">Объект спецификации, описывающий критерии выборки.</param>
        /// <returns>
        /// Сформированный запрос <see cref="IQueryable{T}"/>, содержащий все <c>Where</c>, <c>Include</c> и другие операторы,
        /// но еще не выполненный в базе данных.
        /// </returns>
        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
        }
    }
}
