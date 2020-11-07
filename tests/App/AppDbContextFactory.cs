using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PublicContacts.App.Contexts;
using PublicContacts.Domain;
using PublicContacts.Persistance.Contexts;

namespace PublicContacts.App.Tests
{
    public class AppDbContextFactory : IDisposable
    {
        private DbConnection? _connection;

        private DbContextOptions<AppDbContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection).Options;
        }

        public IAppDbContext CreateContext(bool seed = default)
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();

                var options = CreateOptions();
                using (var context = new AppDbContext(options))
                {
                    context.Database.EnsureCreated();
                    if (seed)
                    {
                        var contacts = new Contact[]
                        {
                            new Contact {
                                Name = "A",
                                Address = "A",
                                DateOfBirth =new DateTime(1990,12,31),
                            },
                            new Contact {
                                Name = "B",
                                Address = "B",
                                DateOfBirth = new DateTime(2000,1,1),
                            },
                        };

                        foreach (var contact in contacts)
                        {
                            contact.PhoneNumbers = new List<PhoneNumber>{
                                new PhoneNumber { Number = $"+38514825309" },
                                new PhoneNumber { Number = $"+385997972327" },
                            };
                            context.Contacts.Add(contact);
                            context.SaveChanges(); // In the loop to ensure proper order of Ids
                        }
                    }
                }
            }

            return new AppDbContext(CreateOptions());
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}