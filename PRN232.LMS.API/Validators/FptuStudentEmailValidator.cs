using System.Text.RegularExpressions;

namespace PRN232.LMS.API.Validators;

public static class FptuStudentEmailValidator
{
    private static readonly Regex FptuStudentCodePattern = new(
        "^(SE|CE|HE|QE|IA|AI)[0-9]{5}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static bool HasValidFptuStudentCodeWhenRequired(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return true;
        }

        var trimmedEmail = email.Trim();
        var atIndex = trimmedEmail.LastIndexOf('@');

        if (atIndex <= 0 || atIndex == trimmedEmail.Length - 1)
        {
            return true;
        }

        var localPart = trimmedEmail[..atIndex];
        var domain = trimmedEmail[(atIndex + 1)..];

        if (!domain.Equals("fpt.edu.vn", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return FptuStudentCodePattern.IsMatch(localPart);
    }
}
