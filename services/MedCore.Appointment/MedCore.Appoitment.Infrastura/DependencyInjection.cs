using MedCore.Appointment.Application.DTOs;
using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Repositories;
using MedCore.Appoitment.Infrastura.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace MedCore.Appoitment.Infrastura;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Employee, EmployeeDto>, EmployeeRepository>();
        services.AddScoped<IRepository<Meet, MeetDto>, MeetRepository>();
        services.AddScoped<IRepository<Skill, SkillDto>, SkillRepository>();
        return services;
    }
}