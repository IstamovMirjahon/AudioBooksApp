namespace AudioBooks.Domain.DTOs;

public class RequestResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class RequestResponseDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
}
