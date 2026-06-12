using System.Collections.Concurrent;
using MedCore.Appoitment.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace MedCore.Appointment.Application.Tests.Helpers
{
    internal static class DbContextFactory
    {
        private static readonly ConcurrentDictionary<string, InMemoryDatabaseRoot> _roots = new();

        public static AppoitmentDbContext Create(string dbName)
        {
            var root = _roots.GetOrAdd(dbName, _ => new InMemoryDatabaseRoot());

            var options = new DbContextOptionsBuilder<AppoitmentDbContext>()
                .UseInMemoryDatabase(dbName, root)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new AppoitmentDbContext(options);
        }
    }
}
