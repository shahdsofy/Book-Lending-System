using Book_Lending_System.Application.Abstraction.DTOs.Identity;
using Book_Lending_System.Application.Abstraction.Services.Identity;
using Book_Lending_System.Application.Features.Identity.Commands.Models;
using Book_Lending_System.Shared.Responses;
using MediatR;

namespace Book_Lending_System.Application.Features.Identity.Commands.Handlers
{
    public class IdentityCommandHandler : 
        IRequestHandler<RegisterCommand, Response<UserDTO>>,
        IRequestHandler<LoginCommand, Response<UserDTO>>,
        IRequestHandler<ConfirmUserEmail, Response<string>>
    {
        private readonly IAuthService authService;

        public IdentityCommandHandler(IAuthService authService)
        {
            this.authService = authService;
        }
        public async Task<Response<UserDTO>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
           return await authService.RegisterAsync(request.RegisterDTO);
        }

        public async Task<Response<UserDTO>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
           return await authService.Login(request.LoginDTO);
        }

        public async Task<Response<string>> Handle(ConfirmUserEmail request, CancellationToken cancellationToken)
        {
            return await authService.ConfirmEmail(request.EmailDTO);
        }
    }
}
