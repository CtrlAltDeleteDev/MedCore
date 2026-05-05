using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using MedCore.Auth.IdentityServer;
using MedCore.DatabaseMigrationJob.Configuration;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MedCore.DatabaseMigrationJob
{
    public class MigrationWorker
    {
        private readonly PersistedGrantDbContext _persistendGrantDbContext;
        private readonly ConfigurationDbContext _configurationDbContext;
        private readonly ILogger _logger;
        private readonly Settings _settings;

        public MigrationWorker(PersistedGrantDbContext persistedGrantDbContext, ConfigurationDbContext configurationDbContext, ILogger logger, Settings settings)
        {
            _persistendGrantDbContext = persistedGrantDbContext;
            _configurationDbContext = configurationDbContext;
            _logger = logger;
            _settings = settings;
        }

        public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Starting database migration proccess...");
                await _persistendGrantDbContext.Database
                    .MigrateAsync(_settings.MigrationSettings.TargetMigration[nameof(PersistedGrantDbContext)], cancellationToken);

                await _configurationDbContext.Database
                    .MigrateAsync(_settings.MigrationSettings.TargetMigration[nameof(ConfigurationDbContext)],cancellationToken);
                Log.Information("Done migrating database");

                if (_settings.SeedData)
                {
                    Log.Information("Seeding database...");
                    await EnsureSeedDataAsync(_configurationDbContext, cancellationToken);
                    Log.Information("Done seeding database. Exiting.");
                }                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while migrate databases");
                return 0;
            }

            return 1;
        }

        private async Task EnsureSeedDataAsync(ConfigurationDbContext context, CancellationToken cancellationToken)
        {
            if (!context.Clients.Any())
            {
                Log.Debug("Clients being populated");
                foreach (var client in Config.Clients.ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                await context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                Log.Debug("Clients already populated");
            }

            if (!context.IdentityResources.Any())
            {
                Log.Debug("IdentityResources being populated");
                foreach (var resource in Config.IdentityResources.ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                await context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                Log.Debug("IdentityResources already populated");
            }

            if (!context.ApiScopes.Any())
            {
                Log.Debug("ApiScopes being populated");
                foreach (var resource in Config.ApiScopes.ToList())
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                await context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                Log.Debug("ApiScopes already populated");
            }

            if (!context.IdentityProviders.Any())
            {
                Log.Debug("OIDC IdentityProviders being populated");
                context.IdentityProviders.Add(new OidcProvider
                {
                    Scheme = "demoidsrv",
                    DisplayName = "IdentityServer",
                    Authority = "https://demo.duendesoftware.com",
                    ClientId = "login",
                }.ToEntity());
                await context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                Log.Debug("OIDC IdentityProviders already populated");
            }
        }
    }
}
