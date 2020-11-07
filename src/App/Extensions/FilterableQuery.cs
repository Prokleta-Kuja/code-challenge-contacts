using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PublicContacts.App.Extensions
{
    public interface IFilterable
    {
        string? Term { get; set; }
    }
    public static class FilterableQuery
    {
        public static IQueryable<T> Filter<T>(this IQueryable<T> query, IFilterable request, params string[] properties)
        {
            if (string.IsNullOrWhiteSpace(request.Term))
                return query;

            var term = request.Term.Contains('%') ?
                request.Term!.ToUpper() :
                $"%{request.Term!.ToUpper()}%";

            var pred = PredicateBuilder.False<T>();
            foreach (var property in properties)
                pred = pred.Or(o => EF.Functions.Like(EF.Property<string>(o, property).ToUpper(), term));

            return query.Where(pred);
        }
    }
}