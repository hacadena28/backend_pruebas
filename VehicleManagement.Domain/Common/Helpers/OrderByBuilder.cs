using System.Linq.Expressions;

namespace VehicleManagement.Domain.Common.Helpers;

public class OrderByBuilder<T>
{
    private readonly List<Func<IQueryable<T>, IOrderedQueryable<T>>> _orderByClauses = new();

    public OrderByBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        _orderByClauses.Add(query => query.OrderBy(keySelector));
        return this;
    }

    public OrderByBuilder<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        _orderByClauses.Add(query => query.OrderByDescending(keySelector));
        return this;
    }

    public Func<IQueryable<T>, IOrderedQueryable<T>> Build()
    {
        return query =>
        {
            IOrderedQueryable<T>? orderedQuery = null;
            foreach (var clause in _orderByClauses)
            {
                orderedQuery = orderedQuery == null ? clause(query) : clause(orderedQuery);
            }
            return orderedQuery ?? query.OrderBy(e => 0);
        };
    }
}

public class OrderByField
{
    public string Campo { get; set; } = string.Empty;
    public bool EsAscendente { get; set; }
}
