using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PublicContacts.Domain;

namespace PublicContacts.App.Contexts
{
    public interface IAppDbContext : IDisposable
    {
        DbSet<Contact> Contacts { get; set; }
        DbSet<PhoneNumber> PhoneNumbers { get; set; }
        DatabaseFacade Database { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}