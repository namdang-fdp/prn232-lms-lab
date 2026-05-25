namespace PRN232.LMS.Services.Exceptions;

public class ServiceException : Exception
{
    public ServiceErrorCode ErrorCode { get; }
    public int StatusCode { get; }
    public Dictionary<string, string>? Errors { get; }

    public ServiceException(
        ServiceErrorCode errorCode,
        string message,
        int statusCode = 400,
        Dictionary<string, string>? errors = null) : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
        Errors = errors;
    }
}
