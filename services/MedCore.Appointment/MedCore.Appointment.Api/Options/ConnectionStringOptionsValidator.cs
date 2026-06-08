using FluentValidation;

namespace MedCore.Appointment.Api.Options
{
    public class ConnectionStringOptionsValidator : AbstractValidator<ConnectionStringOptions>
    {
        public ConnectionStringOptionsValidator()
        {
            RuleFor(x => x.DefaultConnection)
                .NotEmpty();
        }
    }
}