namespace PRN232.LMS.API.Common.Exceptions;

public enum ErrorCode
{
    Success = 1000,
    ValidationError = 4000,
    MalformedJson = 4001,
    InvalidInput = 4002,
    Unauthenticated = 4010,
    ForbiddenAction = 4030,
    ResourceNotFound = 4040,
    DuplicateResource = 4090,
    UnexpectedError = 5000
}