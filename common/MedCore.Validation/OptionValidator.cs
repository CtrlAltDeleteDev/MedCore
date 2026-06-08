using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MedCore.Validation;

internal class OptionValidator<TOptions> : IValidateOptions<TOptions>
    where TOptions : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _name;

    public OptionValidator(IServiceProvider serviceProvider, string name)
    {
        _serviceProvider = serviceProvider;
        _name = name;
    }

    public ValidateOptionsResult Validate(string name, TOptions options)
    {
        if (string.IsNullOrEmpty(name) || name != _name)
        {
            return ValidateOptionsResult.Success;
        }

        var validator = _serviceProvider.GetRequiredService<FluentValidation.IValidator<TOptions>>();

        var result = validator.Validate(options);

        if (result.IsValid)
        {
            return ValidateOptionsResult.Success;
        }

        var errorMessages = result
            .Errors
            .Select(error => $"{_name}.{error.PropertyName}: {error.ErrorMessage}")
            .ToList();

        return ValidateOptionsResult.Fail(errorMessages);
    }
}