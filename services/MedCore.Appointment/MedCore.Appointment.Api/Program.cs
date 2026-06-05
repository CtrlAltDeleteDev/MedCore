using MedCore.Appointment.Api;
using MedCore.Appointment.Application;
using MedCore.Appoitment.Data;
using MedCore.Appoitment.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddAppointmentData(builder.Configuration);
builder.Services.AddValidators();
builder.Services.AddRepositories();

var app = builder.Build();

app.ConfigurePipeline();

app.Run();
