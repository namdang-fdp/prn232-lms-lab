using FluentValidation;
using PRN232.LMS.API.Models.Requests;

namespace PRN232.LMS.API.Validators;

public class CreateStudentRequestValidator : AbstractValidator<CreateStudentRequest>
{
    public CreateStudentRequestValidator()
    {
        RuleFor(request => request.FullName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(120)
            .Must(HaveAtLeastTwoWords)
            .WithMessage("Full name must contain at least two words.");

        RuleFor(request => request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(150)
            .Must(FptuStudentEmailValidator.HasValidFptuStudentCodeWhenRequired)
            .WithMessage("FPTU student email local-part must follow a code format like SE19886 or CE18793.");

        RuleFor(request => request.DateOfBirth)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(NotBeInFuture)
            .WithMessage("Date of birth must not be in the future.")
            .Must(RepresentReasonableStudentAge)
            .WithMessage("Student age must be between 16 and 100.");
    }

    private static bool HaveAtLeastTwoWords(string? value)
    {
        return !string.IsNullOrWhiteSpace(value) &&
               value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length >= 2;
    }

    private static bool NotBeInFuture(DateOnly dateOfBirth)
    {
        return dateOfBirth <= DateOnly.FromDateTime(DateTime.UtcNow);
    }

    private static bool RepresentReasonableStudentAge(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth > today.AddYears(-age))
        {
            age--;
        }

        return age is >= 16 and <= 100;
    }
}

public class UpdateStudentRequestValidator : AbstractValidator<UpdateStudentRequest>
{
    public UpdateStudentRequestValidator()
    {
        RuleFor(request => request.FullName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(120)
            .Must(HaveAtLeastTwoWords)
            .WithMessage("Full name must contain at least two words.");

        RuleFor(request => request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(150)
            .Must(FptuStudentEmailValidator.HasValidFptuStudentCodeWhenRequired)
            .WithMessage("FPTU student email local-part must follow a code format like SE19886 or CE18793.");

        RuleFor(request => request.DateOfBirth)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(NotBeInFuture)
            .WithMessage("Date of birth must not be in the future.")
            .Must(RepresentReasonableStudentAge)
            .WithMessage("Student age must be between 16 and 100.");
    }

    private static bool HaveAtLeastTwoWords(string? value)
    {
        return !string.IsNullOrWhiteSpace(value) &&
               value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length >= 2;
    }

    private static bool NotBeInFuture(DateOnly dateOfBirth)
    {
        return dateOfBirth <= DateOnly.FromDateTime(DateTime.UtcNow);
    }

    private static bool RepresentReasonableStudentAge(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth > today.AddYears(-age))
        {
            age--;
        }

        return age is >= 16 and <= 100;
    }
}
