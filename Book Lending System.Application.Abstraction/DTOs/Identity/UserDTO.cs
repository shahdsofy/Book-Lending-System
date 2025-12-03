namespace Book_Lending_System.Application.Abstraction.DTOs.Identity
{
    public class UserDTO
    {
        public required string Id { get; set; }
        public required string DisplayName { get; set; }
        public required string Email { get; set; }
        public string? Token { get; set; }
    }
}
