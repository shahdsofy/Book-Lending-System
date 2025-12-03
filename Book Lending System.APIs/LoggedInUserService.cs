using Book_Lending_System.Application.Abstraction;
using System.Security.Claims;

namespace Book_Lending_System.APIs
{
    public class LoggedInUserService : ILoggedInUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggedInUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
           // UserId= _httpContextAccessor.HttpContext?.User.FindFirstValue( ClaimTypes.PrimarySid);
        }
        public string? UserId
        {
            get
            {
                return _httpContextAccessor.HttpContext?
                    .User?.FindFirstValue(ClaimTypes.PrimarySid);
            }
        }
    }
}
