var builder = DistributedApplication.CreateBuilder(args);

var jwtIssuer = builder.AddParameter("jwt-issuer");
var jwtAudience = builder.AddParameter("jwt-audience");
var jwtSecretKey = builder.AddParameter("jwt-secret-key", secret: true);
var jwtExpirationMinutes = builder.AddParameter("jwt-expiration-minutes");

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
    .WithEnvironment("JwtSettings__Issuer", jwtIssuer)
    .WithEnvironment("JwtSettings__Audience", jwtAudience)
    .WithEnvironment("JwtSettings__SecretKey", jwtSecretKey)
    .WithEnvironment("JwtSettings__ExpirationMinutes", jwtExpirationMinutes)
    .WaitFor(migrator);

builder.AddProject<Projects.MedCore_Appointment_Api>("medcore-appointment-api")
    .WithEnvironment("ConnectionStrings__DefaultConnection", sqlDb.Resource.ConnectionStringExpression)
    .WithEnvironment("JwtSettings__Issuer", jwtIssuer)
    .WithEnvironment("JwtSettings__Audience", jwtAudience)
    .WithEnvironment("JwtSettings__SecretKey", jwtSecretKey)
    .WithEnvironment("JwtSettings__ExpirationMinutes", jwtExpirationMinutes)
    .WaitFor(migrator);

await builder.Build().RunAsync();
