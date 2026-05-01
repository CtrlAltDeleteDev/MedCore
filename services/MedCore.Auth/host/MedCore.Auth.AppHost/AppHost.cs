var builder = DistributedApplication.CreateBuilder(args);

var sql = builder
    .AddSqlServer("db")
    .WithImageTag("2025-latest")
    .WithLifetime(ContainerLifetime.Persistent);

var sqlDb = sql.AddDatabase("sqldb");

builder
    .AddProject<Projects.MedCore_Auth_IdentityServer>("medcore-auth-identityserver")
    .WithArgs("/seed")
    .WithEnvironment("ConnectionStrings__DefaultConnection", sqlDb.Resource.ConnectionStringExpression)
    .WaitFor(sqlDb);

await builder.Build().RunAsync();
