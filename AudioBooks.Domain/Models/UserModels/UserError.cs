using AudioBooks.Domain.Abstractions;

namespace AudioBooks.Domain.Models.UserModels;

public static class UserError
{
    public static Error ConfirmPassword = new(
        "ConfirmPassword.Failed",
        "Tasdiqlash paroli xato kiritilgan");

    public static Error VerificationCode = new(
        "VerificationCode.Failed",
        "Tasdiqlash kodi eskirgan, qayta jo'nating");

    public static Error CheckEmail = new(
        "CheckEmail.Failed",
        "Bu email oldin ro'yhatdan o'tgan");

    public static Error NotFound = new(
       "Users.NotFound",
       "Users jadvali bo'sh");

    public static Error UserNotFound = new(
       "User.NotFound",
       "Bunday user mavjud emas");

    public static Error EmailNotFound = new(
        "EmailNotFound.Failed",
        "Bunday email mavjud emas");

    public static Error SignIn = new(
        "SignIn.Failed",
        "Kirish muvaffaqqiyatsiz");

    public static Error OldPassword = new(
        "OldPassword.Failed",
        "Eski parol xato");

    public static Error UnFollow = new(
        "UnFollow.Failed",
        "Bu userda obuna bo'lmagansiz");

    public static Error Follow = new(
        "UnFollow.Failed",
        "Bu userga oldin obuna bo'lgansiz");
    public static Error UserInterest = new(
        "Null",
        "Qiziqishlaringiz yuq ");
}
