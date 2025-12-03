using Book_Lending_System.Shared.Errors;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Book_Lending_System.Shared.Responses
{
    public class ResponseHandler
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ResponseHandler(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public Response<T> Success<T>(T entity,string message="Success",object meta=null!)
        {
            var context = httpContextAccessor.HttpContext;
            context.Response.StatusCode =(int) HttpStatusCode.OK;

            return Response<T>.Success(entity, message, null, meta);
        }

        public Response<T> Fail<T>(string message, ErrorType errorType = ErrorType.Unexpected, object meta = null!)
        {
            HttpStatusCode statusCode = errorType switch
            {
                ErrorType.NotFound => HttpStatusCode.NotFound,
                ErrorType.Unauthorized => HttpStatusCode.Unauthorized,
                ErrorType.Validation => HttpStatusCode.BadRequest,
                ErrorType.Forbidden => HttpStatusCode.Forbidden,
                ErrorType.Conflict => HttpStatusCode.Conflict,
                _ => HttpStatusCode.BadRequest
            };

            var context = httpContextAccessor.HttpContext;
            context.Response.StatusCode = (int)statusCode;
            return Response<T>.Fail(statusCode, errorType.ToString(), message, new List<string> { message }, meta);
        }
    }
}
