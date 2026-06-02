using FluentValidation;
using MedCore.Appointment.Application.Commands.CreateNewMeet;
using MedCore.Appointment.Application.Queries.GetDoctorQuery;
using MedCore.Appointment.Application.Queries.GetDoctorsBySkillQuery;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Appointment.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ApplicationAssemblyMarker>());
            return services;
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<GetDoctorsBySkillQuery>, GetDoctorsBySkillQueryValidator>();
            services.AddScoped<IValidator<GetDoctorMeetsQuery>, GetDoctorMeetsQueryValidator>();
            services.AddScoped<IValidator<CreateNewMeetCommand>, CreateNewMeetCommandValidator>();
            return services;
        }
    }
}
