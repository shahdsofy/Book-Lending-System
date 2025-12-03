using System.Net;

namespace Book_Lending_System.Shared.Responses
{
    public class Response<T>
    {
        public bool Succeeded { get; set; }
        public T? Data { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; }=string.Empty;

        public int? Count { get; set; }

        public object? Meta { get; set; }

        public string ErrorType { get; set; }=string.Empty;

        public List<string> Errors { get; set; } = new();

        public static Response<T> Success(T data,  string message = "Success", int? count = null, object? meta = null)
        {
            return new Response<T>
            {
                Succeeded = true,
                Data = data,
                StatusCode = HttpStatusCode.OK,
                Message = message,
                Count = count,
                Meta = meta,
                Errors = new List<string>(),
                ErrorType = "None"
            };
        }
        public static Response<T> Fail(HttpStatusCode statusCode, string errorType, string message, List<string> errors = null!, object meta = null!)
        {
            return new Response<T>
            {
                Succeeded = false,
                StatusCode = statusCode,
                Data = default!,
                Count = null,
                Message = message,
                ErrorType = errorType,
                Errors = errors ?? new List<string> { message },
                Meta = meta
            };
        }
    }
}
