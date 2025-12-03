namespace Book_Lending_System.Application.Abstraction.DTOs.Identity
{
    public class jwtSettings
    {
        public string Key { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int DurationsinMinutes { get; set; }

    }
}
