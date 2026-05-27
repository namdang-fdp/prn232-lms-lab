namespace PRN232.LMS.API.Common.Exceptions;

public class ApiException : Exception
{
    public ErrorCode ErrorCode { get; }
    public int StatusCode { get; }
vvvvvvvvvvvvvvvv
    public ApiException(ErrorCode errorCode, string message, int statusCode = 400) : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}