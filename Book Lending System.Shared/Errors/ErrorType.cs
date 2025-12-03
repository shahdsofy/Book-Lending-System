namespace Book_Lending_System.Shared.Errors
{
    public enum ErrorType
    {
        None,
        Validation,
        NotFound,
        BadRequest,
        Conflict,
        Unauthorized,
        Forbidden,
        Unexpected
    }
}
