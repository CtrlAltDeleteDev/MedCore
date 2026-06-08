using Microsoft.Extensions.DependencyInjection;

namespace MedCore.Validation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOptionsWithValidation<TOptions>(
        this IServiceCollection services,
        string configurationSection)
        where TOptions : class
    {
        services.AddOptions<TOptions>()
            .BindConfiguration(configurationSection)
            .ValidateFluentValidation()
            .ValidateOnStart();

        return services;
    }
}