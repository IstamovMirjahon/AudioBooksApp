using System.ComponentModel.DataAnnotations;

namespace AudioBooks.Domain.Models.DTOs.Auth;

public class UserRegisterDto
{
    [Required(ErrorMessage = "Email manzilini kiritish shart.")]
    [EmailAddress(ErrorMessage = "Email manzil noto'g'ri formatda.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Parolni kiritish shart.")]
    [StringLength(15, MinimumLength = 8, ErrorMessage = "Parol kamida 8 ta belgidan iborat bo'lishi kerak. Maksimum: 15.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
        ErrorMessage = "Parol kamida 1 ta kichik harf, 1 ta katta harf, 1 ta belgi va 1 ta raqamdan iborat bo'lishi kerak.")]
    public string Password { get; set; }
    [Required(ErrorMessage = "Parolni kiritish shart.")]
    [StringLength(15, MinimumLength = 8, ErrorMessage = "Parol kamida 8 ta belgidan iborat bo'lishi kerak. Maksimum: 15.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
        ErrorMessage = "Parol kamida 1 ta kichik harf, 1 ta katta harf, 1 ta belgi va 1 ta raqamdan iborat bo'lishi kerak.")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Tug'ilgan sanani kiritish shart.")]
    [DataType(DataType.DateTime, ErrorMessage = "Tug'ilgan sana noto'g'ri formatda.")]
    public DateTime DateOfBirth { get; set; }
}