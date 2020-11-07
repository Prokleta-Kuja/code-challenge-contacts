using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PublicContacts.App.Contexts;
using PublicContacts.Domain;
using Serilog;
using Serilog.Events;

namespace PublicContacts.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = CreateLogger();

            try
            {
                Log.Information("Application starting up");
                var host = CreateHostBuilder(args).Build();

                using var scope = host.Services.CreateScope();
                var services = scope.ServiceProvider;
                InitializeDb(services);

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.Information("Application shut down");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        static Serilog.ILogger CreateLogger()
        {
            var conf = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext:l}] {Message:lj}{NewLine}{Exception}");

            if (Debugger.IsAttached)
                conf.MinimumLevel.Debug();
            else
                conf
                    .MinimumLevel.Information()
                    .MinimumLevel.Override(nameof(Microsoft), LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information);

            return conf.CreateLogger();
        }

        static void InitializeDb(IServiceProvider services)
        {
            var db = services.GetRequiredService<IAppDbContext>();

            if (db.Database.IsSqlite())
                Log.Information("No Postgres connection string provided, using Sqlite database provider");
            else
                Log.Information("Using Npgsql database provider");

            if (!db.Database.CanConnect() && !db.Database.GetMigrations().Any())
                db.Database.EnsureCreated();
            else if (db.Database.GetMigrations().Any())
                db.Database.Migrate();

            if (db.Contacts.Any())
                return;

            Log.Information("Seeding database");
            var rand = new Random();

            var numbers = DemoData.GetNumbers();
            var contacts = DemoData.GetContacts();
            foreach (var contact in contacts)
            {
                contact.PhoneNumbers = new List<PhoneNumber>{
                    new PhoneNumber {Number = numbers[rand.Next(0,999)] },
                    new PhoneNumber {Number = numbers[rand.Next(0,999)] },
                    new PhoneNumber {Number = numbers[rand.Next(0,999)] },
                    new PhoneNumber {Number = numbers[rand.Next(0,999)] },
                };
                db.Contacts.Add(contact);
            }

            db.SaveChanges();
            Log.Debug("Database seeding complete");
        }
    }
}
