using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Repositories;
using MedCore.Appoitment.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace MedCore.Appoitment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Employee>, EmployeeRepository>();
        services.AddScoped<IRepository<Meet>, MeetRepository>();
        services.AddScoped<IRepository<Skill>, SkillRepository>();
        return services;
    }
}