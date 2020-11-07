using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PublicContacts.App.Extensions
{
    public interface ISortable
    {
        string? SortBy { get; set; }
        bool Ascending { get; set; }
    }
    public static class SortableQuery
    {
        public static IQueryable<T> Sort<T>(this IQueryable<T> query, ISortable request, string? defaultProperty = default)
        {
            string? property = null;
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                // Json formatters converts PascalCase to camelCase so we need to put it back to PascalCase
                char[] a = request.SortBy!.ToCharArray();
                a[0] = char.ToUpper(a[0]);
                property = new string(a);
            }
            else if (!string.IsNullOrWhiteSpace(defaultProperty))
                property = defaultProperty;
            else
                return query;

            if (typeof(T).GetProperty(property) == null)
                return query;

            return query = request.Ascending ?
                  query.OrderBy(o => EF.Property<object>(o, property)) :
                  query.OrderByDescending(o => EF.Property<object>(o, property));
        }
    }
}