using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MedCore.Validation
{
    internal static class OptionsBuilderExtensions
    {
        public static OptionsBuilder<TOptions> ValidateFluentValidation<TOptions>(
            this OptionsBuilder<TOptions> builder)
            where TOptions : class
        {
            builder.Services.AddSingleton<IValidateOptions<TOptions>>(serviceProvider => new OptionValidator<TOptions>(
                serviceProvider,
                builder.Name));

            return builder;
        }
    }
}