using Duende.IdentityModel;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using MedCore.Auth.IdentityServer;
using MedCore.Auth.IdentityServer.Data;
using MedCore.Auth.IdentityServer.Models;
using MedCore.DatabaseMigrationJob.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Security.Claims;

namespace MedCore.DatabaseMigrationJob
{
    public class MigrationWorker
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger _logger;
        private readonly Settings _settings;
        private readonly UserManager<ApplicationUser> _userMgr;

        public MigrationWorker(ApplicationDbContext applicationDbContext, ILogger logger, Settings settings, UserManager<ApplicationUser> userMgr)
        {
            _applicationDbContext = applicationDbContext;
            _logger = logger;
            _settings = settings;
            _userMgr = userMgr;
        }

        public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Starting database migration proccess...");
                await _applicationDbContext.Database
                    .MigrateAsync(_settings.MigrationSettings.TargetMigration[nameof(ApplicationDbContext)], cancellationToken);
                Log.Information("Done migrating database");

                if (_settings.SeedData)
                {
                    Log.Information("Seeding database...");
                    await EnsureSeedData(_applicationDbContext, _userMgr, cancellationToken);
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

        public async Task EnsureSeedData(ApplicationDbContext context, UserManager<ApplicationUser> userMgr, CancellationToken cancellationToken)
        {
            var alice = await userMgr.FindByNameAsync("alice");
            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    UserName = "alice",
                    Email = "AliceSmith@example.com",
                    EmailConfirmed = true,
                };
                var result = await userMgr.CreateAsync(alice, "Pass123$");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(alice, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.example.com"),
                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("alice created");
            }
            else
            {
                Log.Debug("alice already exists");
            }

            var bob = await userMgr.FindByNameAsync("bob");
            if (bob == null)
            {
                bob = new ApplicationUser
                {
                    UserName = "bob",
                    Email = "BobSmith@example.com",
                    EmailConfirmed = true
                };
                var result = await userMgr.CreateAsync(bob, "Pass123$");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(bob, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Bob"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://bob.example.com"),
                            new Claim("location", "somewhere")
                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("bob created");
            }
            else
            {
                Log.Debug("bob already exists");
            }

        }

    }
}
