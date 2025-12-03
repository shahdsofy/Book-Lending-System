using Book_Lending_System.Application.Abstraction.DTOs.Email;
using Book_Lending_System.Application.Abstraction.DTOs.Identity;
using Book_Lending_System.Application.Abstraction.Services.Email;
using Book_Lending_System.Application.Abstraction.Services.Identity;
using Book_Lending_System.Core.Entities.Identity;
using Book_Lending_System.Shared.Errors;
using Book_Lending_System.Shared.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Book_Lending_System.Application.Services.Identity
{
    public class AuthService(UserManager<ApplicationUser>userManager
        ,SignInManager<ApplicationUser>signInManager
        ,IEmailService emailService
        ,IOptions<jwtSettings> _jwtSettings) : IAuthService
    {
        private readonly jwtSettings jwtSettings = _jwtSettings.Value;
        public async Task<Response<UserDTO>> RegisterAsync(RegisterDTO registerDTO)
        {
            var user=new ApplicationUser
            {
                UserName=registerDTO.UserName,
                Email=registerDTO.Email,
                DisplayName=registerDTO.DisplayName,
                PhoneNumber=registerDTO.PhoneNumber,
            };

            var result= await userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
                return Response<UserDTO>.Fail(HttpStatusCode.BadRequest, ErrorType.Validation.ToString(), "Invalid User!");

            await userManager.AddToRoleAsync(user, "Member");

            #region Generate OTP and expiration claim
            var otp = new Random().Next(100000, 999999).ToString();
            var expiration = DateTime.Now.AddMinutes(10);

            await userManager.AddClaimAsync(user, new Claim("email_otp", otp));
            await userManager.AddClaimAsync(user, new Claim("email_otp_expiration", expiration.ToString()));

            var emailMessage = $"Your OTP is {otp}\n. It will expire at {expiration} UTC.";

            var email = new email()
            {
                Body = emailMessage,
                Subject = "Email Confirmation OTP",
                To = registerDTO.Email,
            };

            await emailService.SendEmailAsync(email);

            #endregion

            var userDTO = new UserDTO
            {
                Id= user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
            };

            return Response<UserDTO>.Success(userDTO);
        }

        public async Task<Response<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                return Response<UserDTO>.Fail(HttpStatusCode.Unauthorized, ErrorType.Unauthorized.ToString(), "Invalid Login!");
            }

            
            var result = await signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, lockoutOnFailure: false);

            if (result.IsNotAllowed)
                return Response<UserDTO>.Fail(HttpStatusCode.Unauthorized, ErrorType.Unauthorized.ToString(), "Account is not confirmed yet.");

            if (result.IsLockedOut)
                return Response<UserDTO>.Fail(HttpStatusCode.Unauthorized, ErrorType.Unauthorized.ToString(), "Account is lock Out.");

            if (!result.Succeeded)
                return Response<UserDTO>.Fail(HttpStatusCode.Unauthorized, ErrorType.Unauthorized.ToString(), "Invalid login");


            var response = new UserDTO()
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email!,
                Token = await GenerateToken(user)

            };


            return Response<UserDTO>.Success(response);

        }


        public async Task<Response<string>> ConfirmEmail(ConfirmEmailDTO emailDTO)
        {
            var user = await userManager.FindByEmailAsync(emailDTO.Email);

            if (user == null)
                return Response<string>.Fail(HttpStatusCode.NotFound, ErrorType.NotFound.ToString(), "User not found.");

            var claims = await userManager.GetClaimsAsync(user);

            var otp = claims.FirstOrDefault(c => c.Type == "email_otp")?.Value;
            var expirationDate = claims.FirstOrDefault(c => c.Type == "email_otp_expiration")?.Value;


            if (otp == null || expirationDate == null)
                return Response<string>.Fail(HttpStatusCode.BadRequest, ErrorType.BadRequest.ToString(), "No OTP found. Please request a new one.");

            if (otp != emailDTO.OTP)
                return Response<string>.Fail(HttpStatusCode.BadRequest, ErrorType.BadRequest.ToString(), "Invalid OTP.");

            if (DateTime.Now > DateTime.Parse(expirationDate))
                return Response<string>.Fail(HttpStatusCode.BadRequest, ErrorType.BadRequest.ToString(), "OTP has expired. Please request a new one.");


            user.EmailConfirmed = true;

            var result = await userManager.UpdateAsync(user);

            await userManager.RemoveClaimAsync(user, new Claim("email_otp", otp));
            await userManager.RemoveClaimAsync(user, new Claim("email_otp_expiration", expirationDate));


            if (result.Succeeded)
                return Response<string>.Success("Email is successfully confirmed.");

            return Response<string>.Fail(HttpStatusCode.BadRequest, ErrorType.BadRequest.ToString(), "Email confirmation failed.");
        }


        private async Task<string> GenerateToken(ApplicationUser user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);

            var rolesAsClaims = new List<Claim>();

            var roles = await userManager.GetRolesAsync(user);

            foreach (var role in roles)
                rolesAsClaims.Add(new Claim(ClaimTypes.Role, role));



            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.PrimarySid,user.Id),
                new Claim(ClaimTypes.Email,user.Email!),
                new Claim(ClaimTypes.GivenName,user.UserName!)
            }.Union(userClaims)
            .Union(rolesAsClaims);

            var symmtricClaims = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));


            var signinCredinatials = new SigningCredentials(symmtricClaims, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken
                (
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.DurationsinMinutes),
                claims: claims,
                signingCredentials: signinCredinatials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
