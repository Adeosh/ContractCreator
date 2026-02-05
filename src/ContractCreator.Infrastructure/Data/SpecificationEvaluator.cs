using ContractCreator.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContractCreator.Infrastructure.Data
{
    /// <summary>
    /// Компонент ("Оценщик"), отвечающий за преобразование абстрактной спецификации <see cref="ISpecification{T}"/>
    /// в конкретный запрос Entity Framework Core <see cref="IQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    public class SpecificationEvaluator<T> where T : class
    {
        /// <summary>
        /// Последовательно применяет правила спецификации (Criteria, Includes, OrderBy, Paging) к входному запросу.
        /// </summary>
        /// <param name="inputQuery">Начальный запрос (обычно <c>DbContext.Set&lt;T&gt;()</c>).</param>
        /// <param name="specification">Спецификация, содержащая правила выборки.</param>
        /// <returns>
        /// Результирующий объект <see cref="IQueryable{T}"/>, готовый к выполнению (материализации) в базе данных.
        /// </returns>
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            query = specification.Includes.Aggregate(query,
                (current, include) => current.Include(include));

            query = specification.IncludeStrings.Aggregate(query,
                (current, include) => current.Include(include));

            if (specification.OrderBy != null)
                query = query.OrderBy(specification.OrderBy);
            else if (specification.OrderByDescending != null)
                query = query.OrderByDescending(specification.OrderByDescending);

            if (specification.IsPagingEnabled)
                query = query.Skip(specification.Skip).Take(specification.Take);

            return query;
        }
    }
}
