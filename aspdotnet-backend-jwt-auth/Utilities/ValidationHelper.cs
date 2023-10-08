using FluentValidation;
using JWTBackendAuth.Models;

namespace JWTBackendAuth.Utilities
{
    public class EmailValidator : AbstractValidator<string>
    {
        public EmailValidator()
        {
            RuleFor(str => str).EmailAddress();
        }
    }
}
