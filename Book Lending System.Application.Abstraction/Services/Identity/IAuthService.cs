using Book_Lending_System.Application.Abstraction.DTOs.Identity;
using Book_Lending_System.Shared.Responses;

namespace Book_Lending_System.Application.Abstraction.Services.Identity
{
    public interface IAuthService
    {
        Task<Response<UserDTO>> RegisterAsync (RegisterDTO registerDTO);
        Task<Response<UserDTO>> Login (LoginDTO loginDTO);
        Task<Response<string>> ConfirmEmail (ConfirmEmailDTO emailDTO);

    }
}
