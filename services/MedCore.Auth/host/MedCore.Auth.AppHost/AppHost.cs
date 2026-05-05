var builder = DistributedApplication.CreateBuilder(args);

var sql = builder
    .AddSqlServer("db")
    .WithImageTag("2025-latest")
    .WithLifetime(ContainerLifetime.Persistent);

var sqlDb = sql.AddDatabase("sqldb");


var migrator = builder.AddProject<Projects.MedCore_DatabaseMigrationJob>("medcore-database-migration-job")
    .WithEnvironment("ConnectionStrings__DefaultConnection", sqlDb.Resource.ConnectionStringExpression)
    .WaitFor(sqlDb);

builder
    .AddProject<Projects.MedCore_Auth_IdentityServer>("medcore-auth-identityserver")
    .WithEnvironment("ConnectionStrings__DefaultConnection", sqlDb.Resource.ConnectionStringExpression)
    .WaitFor(migrator);

await builder.Build().RunAsync();
