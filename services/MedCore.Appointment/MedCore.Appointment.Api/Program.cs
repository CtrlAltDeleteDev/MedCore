using MedCore.Appointment.Api;
using MedCore.Appointment.Application;
using MedCore.Appoitment.Data;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddAppointmentData(builder.Configuration);
builder.Services.AddValidators();

var app = builder.Build();

app.ConfigurePipeline();

app.Run();
