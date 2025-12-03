using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Book_Lending_System.Application.Abstraction.DTOs.Email;
using Book_Lending_System.Application.Abstraction.DTOs.Identity;
using Book_Lending_System.Application.Abstraction.Services.Email;
using Book_Lending_System.Application.Services.Identity;
using Book_Lending_System.Core.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Book_Lending_System.Tests.IdentityControllerTests
{
    public class AuthServiceSimpleTests
    {
        private readonly IEmailService _emailService = Substitute.For<IEmailService>();
        private readonly jwtSettings _jwtSettings = new jwtSettings
        {
            Key = "test_key_very_long_for_unit_tests_1234567890",
            Issuer = "unit",
            Audience = "unit",
            DurationsinMinutes = 60
        };

        private AuthService CreateService(FakeUserManager userManager, FakeSignInManager signInManager)
            => new AuthService(userManager, signInManager, _emailService, Microsoft.Extensions.Options.Options.Create(_jwtSettings));

        [Fact]
        public async Task Register_Succeeds_And_Sends_Email()
        {
            //Arrange
            var userManager = new FakeUserManager();
            userManager.CreateAsyncImpl = (u, p) => Task.FromResult(IdentityResult.Success);

            var signInManager = new FakeSignInManager(userManager);
            var svc = CreateService(userManager, signInManager);

            var dto = new RegisterDTO
            {
                DisplayName = "Disp",
                UserName = "user1",
                Email = "user1@example.com",
                Password = "P@ssw0rd!",
                PhoneNumber = "123"
            };

            //Act
            var res = await svc.RegisterAsync(dto);

            //Assert
            Assert.True(res.Succeeded);
            Assert.Equal(dto.Email, res.Data!.Email);
            await _emailService.Received(1).SendEmailAsync(Arg.Any<email>());

            // OTP claim was added to our fake store
            var claims = await userManager.GetClaimsAsync(new ApplicationUser { Email = dto.Email, UserName = dto.UserName, DisplayName = dto.DisplayName });
            Assert.Contains(claims, c => c.Type == "email_otp");
        }

        [Fact]
        public async Task Register_Fails_When_CreateAsync_Fails()
        {
            var userManager = new FakeUserManager();
            userManager.CreateAsyncImpl = (u, p) => Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "bad" }));

            var signInManager = new FakeSignInManager(userManager);
            var svc = CreateService(userManager, signInManager);

            var dto = new RegisterDTO
            {
                DisplayName = "Disp",
                UserName = "user2",
                Email = "user2@example.com",
                Password = "bad",
                PhoneNumber = "123"
            };

            var res = await svc.RegisterAsync(dto);

            Assert.False(res.Succeeded);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, res.StatusCode);
        }

        [Fact]
        public async Task Login_Returns_Token_When_Credentials_Are_Valid()
        {
            var user = new ApplicationUser { Id = "u1", UserName = "user", Email = "u@example.com", DisplayName = "Display" };

            var userManager = new FakeUserManager();
            userManager.FindByEmailAsyncImpl = email => Task.FromResult<ApplicationUser?>(user);
            userManager.GetClaimsImpl = u => Task.FromResult<IList<Claim>>(new List<Claim>());
            userManager.GetRolesImpl = u => Task.FromResult<IList<string>>(new List<string>());

            var signInManager = new FakeSignInManager(userManager);
            signInManager.CheckPasswordSignInAsyncImpl = (u, p, l) => Task.FromResult(SignInResult.Success);

            var svc = CreateService(userManager, signInManager);

            var res = await svc.Login(new LoginDTO { Email = user.Email, Password = "correct" });

            Assert.True(res.Succeeded);
            Assert.False(string.IsNullOrWhiteSpace(res.Data!.Token));
            Assert.Equal(user.Email, res.Data.Email);
        }

        [Fact]
        public async Task ConfirmEmail_Succeeds_With_Valid_OTP()
        {
            var user = new ApplicationUser { Id = "c1", UserName = "cu", Email = "c1@example.com", DisplayName = "C" };
            var otp = "555555";
            var expiration = DateTime.Now.AddMinutes(5).ToString();

            var userManager = new FakeUserManager();
            userManager.FindByEmailAsyncImpl = email => Task.FromResult<ApplicationUser?>(user);
            userManager.GetClaimsImpl = u => Task.FromResult<IList<Claim>>(new List<Claim>
            {
                new Claim("email_otp", otp),
                new Claim("email_otp_expiration", expiration)
            });
            userManager.UpdateAsyncImpl = u => Task.FromResult(IdentityResult.Success);

            var signInManager = new FakeSignInManager(userManager);
            var svc = CreateService(userManager, signInManager);

            var res = await svc.ConfirmEmail(new ConfirmEmailDTO { Email = user.Email, OTP = otp });

            Assert.True(res.Succeeded);
            Assert.Equal("Email is successfully confirmed.", res.Data);
        }

        // --- small helpers: lightweight fakes only for the methods used in AuthService ---
        private class FakeUserManager : UserManager<ApplicationUser>
        {
            public Func<ApplicationUser, string, Task<IdentityResult>> CreateAsyncImpl { get; set; } = (u, p) => Task.FromResult(IdentityResult.Success);
            public Func<string, Task<ApplicationUser?>> FindByEmailAsyncImpl { get; set; } = _ => Task.FromResult<ApplicationUser?>(null);
            public Func<ApplicationUser, Task<IdentityResult>> UpdateAsyncImpl { get; set; } = _ => Task.FromResult(IdentityResult.Success);
            public Func<ApplicationUser, Task<IList<Claim>>> GetClaimsImpl { get; set; } = _ => Task.FromResult<IList<Claim>>(new List<Claim>());
            public Func<ApplicationUser, Task<IList<string>>> GetRolesImpl { get; set; } = _ => Task.FromResult<IList<string>>(new List<string>());

            private readonly Dictionary<string, List<Claim>> _claims = new();
            private readonly Dictionary<string, List<string>> _roles = new();

            public FakeUserManager()
                : base(Substitute.For<IUserStore<ApplicationUser>>(),
                      Microsoft.Extensions.Options.Options.Create(new IdentityOptions()),
                      Substitute.For<IPasswordHasher<ApplicationUser>>(),
                      Array.Empty<IUserValidator<ApplicationUser>>(),
                      Array.Empty<IPasswordValidator<ApplicationUser>>(),
                      Substitute.For<ILookupNormalizer>(),
                      new IdentityErrorDescriber(),
                      Substitute.For<IServiceProvider>(),
                      Substitute.For<ILogger<UserManager<ApplicationUser>>>())
            { }

            public override Task<IdentityResult> CreateAsync(ApplicationUser user, string password) => CreateAsyncImpl(user, password);

            public override Task<ApplicationUser?> FindByEmailAsync(string email) => FindByEmailAsyncImpl(email);

            public override Task<IdentityResult> UpdateAsync(ApplicationUser user) => UpdateAsyncImpl(user);

            public override async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
            {
                var key = user.Email ?? user.Id ?? Guid.NewGuid().ToString();
                if (_claims.TryGetValue(key, out var list))
                    return list.ToList();

                var impl = await GetClaimsImpl(user);
                return impl.ToList();
            }

            public override Task<IdentityResult> AddClaimAsync(ApplicationUser user, Claim claim)
            {
                var key = user.Email ?? user.Id ?? Guid.NewGuid().ToString();
                if (!_claims.ContainsKey(key)) _claims[key] = new List<Claim>();
                _claims[key].Add(claim);
                return Task.FromResult(IdentityResult.Success);
            }

            public override Task<IdentityResult> RemoveClaimAsync(ApplicationUser user, Claim claim)
            {
                var key = user.Email ?? user.Id ?? Guid.NewGuid().ToString();
                if (_claims.TryGetValue(key, out var list))
                {
                    var toRemove = list.FirstOrDefault(c => c.Type == claim.Type && c.Value == claim.Value);
                    if (toRemove != null) list.Remove(toRemove);
                }
                return Task.FromResult(IdentityResult.Success);
            }

            public override Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
            {
                var key = user.Email ?? user.Id ?? Guid.NewGuid().ToString();
                if (!_roles.ContainsKey(key)) _roles[key] = new List<string>();
                if (!_roles[key].Contains(role)) _roles[key].Add(role);
                return Task.FromResult(IdentityResult.Success);
            }

            public override Task<IList<string>> GetRolesAsync(ApplicationUser user)
            {
                var key = user.Email ?? user.Id ?? Guid.NewGuid().ToString();
                var stored = _roles.TryGetValue(key, out var list) ? list.ToList() : new List<string>();
                return Task.FromResult<IList<string>>(stored);
            }
        }

        private class FakeSignInManager : SignInManager<ApplicationUser>
        {
            public Func<ApplicationUser, string, bool, Task<SignInResult>> CheckPasswordSignInAsyncImpl { get; set; } =
                (u, p, l) => Task.FromResult(SignInResult.Failed);

            public FakeSignInManager(UserManager<ApplicationUser> userManager)
                : base(userManager,
                      Substitute.For<IHttpContextAccessor>(),
                      Substitute.For<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                      Microsoft.Extensions.Options.Options.Create(new IdentityOptions()),
                      Substitute.For<ILogger<SignInManager<ApplicationUser>>>(),
                      Substitute.For<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>(),
                      Substitute.For<IUserConfirmation<ApplicationUser>>())
            { }

            public override Task<SignInResult> CheckPasswordSignInAsync(ApplicationUser user, string password, bool lockoutOnFailure)
                => CheckPasswordSignInAsyncImpl(user, password, lockoutOnFailure);
        }
    }
}