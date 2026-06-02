cd MedCore\services\MedCore.Auth\src\MedCore.Auth.IdentityServer

dotnet ef migrations add InitialMigration -c ApplicationDbContext -o Data/Migrations
