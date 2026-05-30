using MedCore.Appointment.Api;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

app.ConfigurePipeline();

app.Run();
