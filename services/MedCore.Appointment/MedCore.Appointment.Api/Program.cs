using MedCore.Appointment.Api;
using MedCore.Appointment.Application;
using MedCore.Appoitment.Data;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddAppoitmentData(builder.Configuration);

var app = builder.Build();

app.ConfigurePipeline();

app.Run();
