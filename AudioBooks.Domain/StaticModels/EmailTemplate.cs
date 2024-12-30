namespace AudioBooks.Domain.StaticModels;

public class EmailTemplate
{
    public static string GetWelcomeEmailTemplate(string email, string fullName)
    {
        return $@"
                  <h2>Hurmatli {fullName},</h2>
                  <p class='user-info'>Muvaffaqqiyatli ro'yhatdan o'tdingiz.</span></p>
                  <p class='user-info'>Sizning foydalanuvchi nomingiz: <span class='highlight'>{email}</span></p>";
    }

    public static string GetVerificationEmailTemplate(int code)
    {
        return $@"
                  <h2>Hurmatli foydalanuvchi,</h2>
                  <p>Sizning tasdiqlash kodingiz:
                  <h3><span class='highlight'>{code}</span></h3></p>
                  <p>Ushbu kodni kiriting va hisobingizni tasdiqlang.</p>";
    }
}
