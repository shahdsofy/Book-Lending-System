namespace Book_Lending_System.Application.Abstraction.DTOs.Identity
{
    public class LoginDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
