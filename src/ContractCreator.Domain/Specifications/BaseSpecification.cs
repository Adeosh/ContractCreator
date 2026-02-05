using ContractCreator.Domain.Interfaces;
using System.Linq.Expressions;

namespace ContractCreator.Domain.Specifications
{
    /// <summary>
    /// Абстрактный базовый класс для создания спецификаций сущностей.
    /// <para>
    /// Спецификация инкапсулирует логику построения запроса к базе данных (фильтрация, сортировка, жадная загрузка, пагинация)
    /// в виде объектов, которые можно переиспользовать и тестировать отдельно от базы данных.
    /// </para>
    /// </summary>
    /// <typeparam name="T">Тип сущности, к которой применяется спецификация.</typeparam>
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        public BaseSpecification() { }

        /// <summary>
        /// Создает спецификацию с заданным критерием фильтрации (Where).
        /// </summary>
        /// <param name="criteria">Выражение (Expression) для фильтрации записей. Например: <c>x => x.Id == id</c>.</param>
        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>> Criteria { get; }
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        public List<string> IncludeStrings { get; } = new();
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; }

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }
    }
}
