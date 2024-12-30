namespace AudioBooks.Domain.Models.UserModels;

public class SignInResponse
{
    public string Massage { get; set; }
    public string BearerToken { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Role { get; set; }
}
