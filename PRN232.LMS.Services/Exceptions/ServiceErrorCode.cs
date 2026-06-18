namespace PRN232.LMS.Services.Exceptions;

public enum ServiceErrorCode
{
    ValidationError = 4000,
    Unauthorized = 4010,
    ResourceNotFound = 4040,
    DuplicateResource = 4090,
    Conflict = 4091
}
