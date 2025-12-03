using Book_Lending_System.Application.Abstraction.DTOs.Identity;
using Book_Lending_System.Shared.Responses;
using MediatR;

namespace Book_Lending_System.Application.Features.Identity.Commands.Models
{
    public class LoginCommand:IRequest<Response<UserDTO>>
    {
        public LoginDTO LoginDTO { get; set; }
    }
}
