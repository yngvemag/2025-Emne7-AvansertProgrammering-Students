using FluentValidation;
using StudentBloggAPI.Features.Users.Dtos;

namespace StudentBloggAPI.Features.Common.Validators;

public class UserRegistrationDtoValidator : AbstractValidator<UserRegistrationDto>
{
    public UserRegistrationDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters long")
            .MaximumLength(50).WithMessage("First name must be less than 50 characters long");
        
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters long")
            .MaximumLength(50).WithMessage("Last name must be less than 50 characters long");
        
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long")
            .MaximumLength(50).WithMessage("Username cannot exceed 50 characters");
        
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email must be a valid email address");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .Length(8, 16).WithMessage("Password must be between 8 and 16 characters long")
            .Must(p => p.Any(char.IsDigit)).WithMessage("Password must contain at least one number")
            //.Matches("[0-9]+").WithMessage("Password must contain at least one number")
            .Matches("[A-Z]+").WithMessage("Password must contain at least one upper case letter")
            .Matches("[a-z]+").WithMessage("Password must contain at least one lower case letter")
            .Matches("[!?*#_]").WithMessage("Password must contain at least one special character '(! ? * # _)'")
            .Must(password => !password.Any(c => "æøåÆØÅ"
                .Contains(c))).WithMessage("Password contains invalid characters.'(æ ø å Æ Ø Å)'");

    }
}