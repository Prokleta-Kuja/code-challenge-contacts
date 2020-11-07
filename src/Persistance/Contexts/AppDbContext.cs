using Microsoft.EntityFrameworkCore;
using PublicContacts.App.Contexts;
using PublicContacts.Domain;

namespace PublicContacts.Persistance.Contexts
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        static readonly string namespacePrefix = $"{nameof(PublicContacts)}.{nameof(Persistance)}.{nameof(ContextConfigurations)}.{nameof(ContextConfigurations.AppDbContext)}";
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Contact> Contacts { get; set; } = null!;
        public DbSet<PhoneNumber> PhoneNumbers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly, t => t.FullName.StartsWith(namespacePrefix));

            //if (Database.IsSqlite())
            // Apply additional Sqlite configurations here
            //if (Database.IsNpgsql())
            // Apply additional Postgres configurations here
        }
    }
}