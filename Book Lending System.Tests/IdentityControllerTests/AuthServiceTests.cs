using Book_Lending_System.Application.Abstraction.DTOs.Email;
using Book_Lending_System.Application.Abstraction.DTOs.Identity;
using Book_Lending_System.Application.Abstraction.Services.Email;
using Book_Lending_System.Application.Services.Identity;
using Book_Lending_System.Core.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Net;
using System.Security.Claims;

namespace Book_Lending_System.Tests.IdentityControllerTests
{
    public class AuthServiceTests
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IOptions<jwtSettings> _jwtOptions;

        public AuthServiceTests()
        {
            var userStore = Substitute.For<IUserStore<ApplicationUser>>();
            _userManager = Substitute.For<UserManager<ApplicationUser>>(
                userStore,
                null,
                new PasswordHasher<ApplicationUser>(),
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null,
                null
            );

            var contextAccessor = Substitute.For<IHttpContextAccessor>();
            var claimsFactory = Substitute.For<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _signInManager = Substitute.For<SignInManager<ApplicationUser>>(
                _userManager,
                contextAccessor,
                claimsFactory,
                null, null, null, null
            );

            _emailService = Substitute.For<IEmailService>();

            _jwtOptions = Options.Create(new jwtSettings
            {
                Key = "SuperSecretTestKey12345SuperSecretTestKey12345",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                DurationsinMinutes = 60
            });
        }

        private AuthService GetService()
        {
            return new AuthService(_userManager, _signInManager, _emailService, _jwtOptions);
        }

        private ApplicationUser CreateTestUser()
        {
            return new ApplicationUser
            {
                Id = "1",
                Email = "test@example.com",
                UserName = "testuser",
                DisplayName = "Test User"
            };
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnSuccess_WhenUserCreated()
        {
            var dto = new RegisterDTO
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "Test@12345",
                DisplayName = "Test User",
                PhoneNumber = "0123456789"
            };

            _userManager.CreateAsync(Arg.Any<ApplicationUser>(), dto.Password).Returns(IdentityResult.Success);
            _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), "Member").Returns(IdentityResult.Success);
            _userManager.AddClaimAsync(Arg.Any<ApplicationUser>(), Arg.Any<Claim>()).Returns(IdentityResult.Success);
            _emailService.SendEmailAsync(Arg.Any<email>()).Returns(Task.CompletedTask);

            var service = GetService();

            var result = await service.RegisterAsync(dto);

            Assert.True(result.Succeeded);
            Assert.Equal(dto.DisplayName, result.Data.DisplayName);
            await _emailService.Received(1).SendEmailAsync(Arg.Any<email>());
        }

        [Fact]
        public async Task RegisterAsync_ShouldFail_WhenCreateFails()
        {
            var dto = new RegisterDTO
            {
                DisplayName = "Test User",
                UserName = "testuser",
                Email = "test@example.com",
                Password = "Test@12345",
                PhoneNumber = "0123456789"
            };

            _userManager.CreateAsync(Arg.Any<ApplicationUser>(), dto.Password)
                .Returns(IdentityResult.Failed(new IdentityError { Description = "Error" }));

            var service = GetService();

            var result = await service.RegisterAsync(dto);

            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Login_ShouldReturnSuccess_WhenPasswordCorrect()
        {
            var user = CreateTestUser();
            var dto = new LoginDTO { Email = user.Email!, Password = "Test@12345" };

            _userManager.FindByEmailAsync(user.Email!).Returns(user);
            _signInManager.CheckPasswordSignInAsync(user, dto.Password, false).Returns(SignInResult.Success);
            _userManager.GetClaimsAsync(user).Returns(new List<Claim>());
            _userManager.GetRolesAsync(user).Returns(new List<string> { "Member" });

            var service = GetService();

            var result = await service.Login(dto);

            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data.Token);
        }

        [Fact]
        public async Task Login_ShouldFail_WhenUserNotFound()
        {
            _userManager.FindByEmailAsync("fake@example.com").Returns((ApplicationUser)null);

            var service = GetService();

            var result = await service.Login(new LoginDTO { Email = "fake@example.com", Password = "123" });

            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task ConfirmEmail_ShouldReturnSuccess_WhenOtpValid()
        {
            var user = CreateTestUser();
            var dto = new ConfirmEmailDTO { Email = user.Email, OTP = "123456" };

            _userManager.FindByEmailAsync(user.Email).Returns(user);
            _userManager.GetClaimsAsync(user).Returns(new List<Claim>
            {
                new Claim("email_otp","123456"),
                new Claim("email_otp_expiration", DateTime.Now.AddMinutes(10).ToString())
            });
            _userManager.UpdateAsync(user).Returns(IdentityResult.Success);
            _userManager.RemoveClaimAsync(user, Arg.Any<Claim>()).Returns(IdentityResult.Success);

            var service = GetService();

            var result = await service.ConfirmEmail(dto);

            Assert.True(result.Succeeded);
            Assert.Equal("Email is successfully confirmed.", result.Data);
        }

        [Fact]
        public async Task ConfirmEmail_ShouldFail_WhenOtpInvalid()
        {
            var user = CreateTestUser();
            var dto = new ConfirmEmailDTO { Email = user.Email, OTP = "000000" };

            _userManager.FindByEmailAsync(user.Email).Returns(user);
            _userManager.GetClaimsAsync(user).Returns(new List<Claim>
            {
                new Claim("email_otp","123456"),
                new Claim("email_otp_expiration", DateTime.Now.AddMinutes(10).ToString())
            });

            var service = GetService();

            var result = await service.ConfirmEmail(dto);

            Assert.False(result.Succeeded);
            Assert.Equal("Invalid OTP.", result.Message);
        }
    }
}
