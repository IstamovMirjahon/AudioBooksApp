namespace AudioBooks.Domain.Abstractions;

public class Error
{
    public Error(string code, string message= "")
    {
        Message = message;
        Code = code;
    }

    public string Code { get; }
    public string Message { get; }
}
