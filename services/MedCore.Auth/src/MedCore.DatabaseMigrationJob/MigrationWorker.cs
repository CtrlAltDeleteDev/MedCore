using Duende.IdentityModel;
using MedCore.Auth.IdentityServer.Data;
using MedCore.Auth.IdentityServer.Models;
using MedCore.DatabaseMigrationJob.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        private async Task EnsureSeedData(ApplicationDbContext context, UserManager<ApplicationUser> userMgr, CancellationToken cancellationToken)
        {
            await EnsureUserAsync(userMgr, "alice", "AliceSmith@example.com", new[]
            {
                new Claim(JwtClaimTypes.Name, "Alice Smith"),
                new Claim(JwtClaimTypes.GivenName, "Alice"),
                new Claim(JwtClaimTypes.FamilyName, "Smith"),
                new Claim(JwtClaimTypes.WebSite, "http://alice.example.com"),
            });

            await EnsureUserAsync(userMgr, "bob", "BobSmith@example.com", new[]
            {
                new Claim(JwtClaimTypes.Name, "Bob Smith"),
                new Claim(JwtClaimTypes.GivenName, "Bob"),
                new Claim(JwtClaimTypes.FamilyName, "Smith"),
                new Claim(JwtClaimTypes.WebSite, "http://bob.example.com"),
                new Claim("location", "somewhere"),
            });

            var patients = new[]
            {
                (id: 1, username: "patient1", email: "melnyk.olha@dental.local",       given: "Ольга",      family: "Мельник"),
                (id: 2, username: "patient2", email: "kovalenko.ivan@dental.local",    given: "Іван",       family: "Коваленко"),
                (id: 3, username: "patient3", email: "bondarenko.yulia@dental.local",  given: "Юлія",       family: "Бондаренко"),
                (id: 4, username: "patient4", email: "tkachenko.dmytro@dental.local",  given: "Дмитро",     family: "Ткаченко"),
                (id: 5, username: "patient5", email: "savchenko.anna@dental.local",    given: "Анна",       family: "Савченко"),
                (id: 6, username: "patient6", email: "kravchenko.serhii@dental.local", given: "Сергій",     family: "Кравченко"),
                (id: 7, username: "patient7", email: "lysenko.natalia@dental.local",   given: "Наталія",    family: "Лисенко"),
                (id: 8, username: "patient8", email: "petrenko.mykola@dental.local",   given: "Микола",     family: "Петренко"),
                (id: 9, username: "patient9", email: "moroz.oksana@dental.local",      given: "Оксана",     family: "Мороз"),
            };

            foreach (var p in patients)
            {
                await EnsureUserAsync(userMgr, p.username, p.email, new[]
                {
                    new Claim(JwtClaimTypes.Name,       $"{p.given} {p.family}"),
                    new Claim(JwtClaimTypes.GivenName,  p.given),
                    new Claim(JwtClaimTypes.FamilyName, p.family),
                    new Claim("patient_id",             p.id.ToString()),
                });
            }
        }

        private async Task EnsureUserAsync(UserManager<ApplicationUser> userMgr, string username, string email, Claim[] claims)
        {
            var user = await userMgr.FindByNameAsync(username);
            if (user != null)
            {
                Log.Debug("{Username} already exists", username);
                return;
            }

            user = new ApplicationUser { UserName = username, Email = email, EmailConfirmed = true };

            var result = await userMgr.CreateAsync(user, "Pass123$");
            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);

            result = await userMgr.AddClaimsAsync(user, claims);
            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);

            Log.Debug("{Username} created", username);
        }

    }
}
