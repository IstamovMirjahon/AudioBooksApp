using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AudioBooks.Domain.CustomAttributes;

public class PasswordValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        string password = value as string;

        // Parol tekshiruvlari
        if (password.Length < 8)
        {
            return new ValidationResult("Password must be at least 8 characters long.");
        }

        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            return new ValidationResult("Password must contain at least 1 uppercase letter.");
        }

        if (!Regex.IsMatch(password, @"[a-z]"))
        {
            return new ValidationResult("Password must contain at least 1 lowercase letter.");
        }

        if (!Regex.IsMatch(password, @"[0-9]"))
        {
            return new ValidationResult("Password must contain at least 1 number.");
        }

        if (!Regex.IsMatch(password, @"[\W_]"))
        {
            return new ValidationResult("Password must contain at least 1 special character.");
        }

        return ValidationResult.Success;
    }
}
