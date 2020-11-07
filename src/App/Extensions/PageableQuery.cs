using System.Linq;

namespace PublicContacts.App.Extensions
{
    public interface IPageable
    {
        int Skip { get; set; }
        int Take { get; set; }
    }
    public static class PageableQuery
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> query, IPageable request)
            => query
                .Skip(request.Skip)
                .Take(request.Take);
    }
}