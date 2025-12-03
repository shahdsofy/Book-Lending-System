namespace Book_Lending_System.Application.Abstraction.DTOs.Identity
{
    public class RegisterDTO
    {
        public required string DisplayName { get; set; }
        public required string UserName { get; set; }
               
        public required string Email { get; set; }
               
        public required string Password { get; set; }
               
        public required string PhoneNumber { get; set; }
               

    }
}
