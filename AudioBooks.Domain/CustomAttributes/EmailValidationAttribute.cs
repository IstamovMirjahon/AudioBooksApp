using System.ComponentModel.DataAnnotations;

namespace AudioBooks.Domain.CustomAttributes;

public class EmailValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        string email = value as string;
        if (email.EndsWith("@gmail.com"))
        {
            return ValidationResult.Success;
        }
        return new ValidationResult("Email address failed");
    }
}
