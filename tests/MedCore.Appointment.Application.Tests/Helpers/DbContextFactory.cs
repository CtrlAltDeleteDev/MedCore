using MedCore.Appoitment.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace MedCore.Appointment.Application.Tests.Helpers
{
    internal static class DbContextFactory
    {
        // Explicit shared roots ensure all contexts with the same DB name
        // truly share one in-memory store across context instances.
        private static readonly Dictionary<string, InMemoryDatabaseRoot> _roots = new();

        public static AppoitmentDbContext Create(string dbName)
        {
            if (!_roots.TryGetValue(dbName, out var root))
            {
                root = new InMemoryDatabaseRoot();
                _roots[dbName] = root;
            }

            var options = new DbContextOptionsBuilder<AppoitmentDbContext>()
                .UseInMemoryDatabase(dbName, root)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new AppoitmentDbContext(options);
        }
    }
}
