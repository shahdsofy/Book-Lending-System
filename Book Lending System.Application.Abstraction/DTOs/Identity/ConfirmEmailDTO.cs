namespace Book_Lending_System.Application.Abstraction.DTOs.Identity
{
    public class ConfirmEmailDTO
    {
        public required string Email { get; set; }
        public required string OTP { get; set; }
    }
}
