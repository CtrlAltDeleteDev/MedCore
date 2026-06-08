using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MedCore.Auth.IdentityServer.Configuration;
using MedCore.Auth.IdentityServer.Controllers.Api;
using MedCore.Auth.IdentityServer.Models;
using MedCore.Auth.IdentityServer.Models.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;
using IdentitySignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace MedCore.Auth.Tests.Controllers;

public sealed class AuthControllerTests
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtSettings _jwtSettings;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            Substitute.For<IUserStore<ApplicationUser>>(),
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);

        _signInManager = Substitute.For<SignInManager<ApplicationUser>>(
            _userManager,
            Substitute.For<IHttpContextAccessor>(),
            Substitute.For<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            null,
            null,
            null,
            null);

        _jwtSettings = new JwtSettings
        {
            Issuer = "test-issuer",
            Audience = "test-audience",
            SecretKey = "test-secret-key-must-be-at-least-32-characters!!",
            ExpirationMinutes = 60
        };

        _controller = new AuthController(
            _signInManager,
            _userManager,
            Options.Create(_jwtSettings),
            Substitute.For<ILogger<AuthController>>());
    }

    [Fact]
    public async Task Login_UserNotFound_Returns401()
    {
        _userManager.FindByNameAsync("unknown").Returns(Task.FromResult<ApplicationUser?>(null));

        var result = await _controller.Login(new LoginRequest { Username = "unknown", Password = "any" });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var user = new ApplicationUser { UserName = "alice" };
        _userManager.FindByNameAsync("alice").Returns(Task.FromResult<ApplicationUser?>(user));
        _signInManager.CheckPasswordSignInAsync(user, "wrong", true)
            .Returns(Task.FromResult(IdentitySignInResult.Failed));

        var result = await _controller.Login(new LoginRequest { Username = "alice", Password = "wrong" });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_AccountLocked_Returns401()
    {
        var user = new ApplicationUser { UserName = "alice" };
        _userManager.FindByNameAsync("alice").Returns(Task.FromResult<ApplicationUser?>(user));
        _signInManager.CheckPasswordSignInAsync(user, "Pass123$", true)
            .Returns(Task.FromResult(IdentitySignInResult.LockedOut));

        var result = await _controller.Login(new LoginRequest { Username = "alice", Password = "Pass123$" });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200()
    {
        var user = BuildUser("alice");
        SetupSuccessfulLogin(user, "Pass123$", roles: []);

        var result = await _controller.Login(new LoginRequest { Username = "alice", Password = "Pass123$" });

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Login_ValidCredentials_ResponseContainsNonEmptyToken()
    {
        var user = BuildUser("alice");
        SetupSuccessfulLogin(user, "Pass123$", roles: []);

        var result = (OkObjectResult)await _controller.Login(new LoginRequest { Username = "alice", Password = "Pass123$" });
        var response = (LoginResponse)result.Value!;

        Assert.NotEmpty(response.AccessToken);
    }

    [Fact]
    public async Task Login_ValidCredentials_ExpiresAtIsInFuture()
    {
        var user = BuildUser("alice");
        SetupSuccessfulLogin(user, "Pass123$", roles: []);

        var result = (OkObjectResult)await _controller.Login(new LoginRequest { Username = "alice", Password = "Pass123$" });
        var response = (LoginResponse)result.Value!;

        Assert.True(response.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_ValidCredentials_TokenContainsUserIdEmailAndUsername()
    {
        var user = BuildUser("alice", id: "user-42", email: "alice@test.com");
        SetupSuccessfulLogin(user, "Pass123$", roles: []);

        var result = (OkObjectResult)await _controller.Login(new LoginRequest { Username = "alice", Password = "Pass123$" });
        var token = ReadToken((LoginResponse)result.Value!);

        Assert.Equal("user-42", token.Subject);
        Assert.Equal("alice", token.Claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value);
        Assert.Equal("alice@test.com", token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
    }

    [Fact]
    public async Task Login_ValidCredentials_TokenIssuedByConfiguredIssuer()
    {
        var user = BuildUser("alice");
        SetupSuccessfulLogin(user, "Pass123$", roles: []);

        var result = (OkObjectResult)await _controller.Login(new LoginRequest { Username = "alice", Password = "Pass123$" });
        var token = ReadToken((LoginResponse)result.Value!);

        Assert.Equal(_jwtSettings.Issuer, token.Issuer);
    }

    [Fact]
    public async Task Login_UserWithRoles_RolesIncludedAsClaimsInToken()
    {
        var user = BuildUser("admin");
        SetupSuccessfulLogin(user, "Pass123$", roles: ["Admin", "Doctor"]);

        var result = (OkObjectResult)await _controller.Login(new LoginRequest { Username = "admin", Password = "Pass123$" });
        var token = ReadToken((LoginResponse)result.Value!);

        var roles = token.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        Assert.Contains("Admin", roles);
        Assert.Contains("Doctor", roles);
    }

    [Fact]
    public async Task Login_UserWithNoRoles_TokenContainsNoRoleClaims()
    {
        var user = BuildUser("alice");
        SetupSuccessfulLogin(user, "Pass123$", roles: []);

        var result = (OkObjectResult)await _controller.Login(new LoginRequest { Username = "alice", Password = "Pass123$" });
        var token = ReadToken((LoginResponse)result.Value!);

        Assert.DoesNotContain(token.Claims, c => c.Type == ClaimTypes.Role);
    }

    private static ApplicationUser BuildUser(string username, string id = "user-id", string email = "user@test.com") =>
        new() { Id = id, UserName = username, Email = email };

    private static JwtSecurityToken ReadToken(LoginResponse response) =>
        new JwtSecurityTokenHandler().ReadJwtToken(response.AccessToken);

    private void SetupSuccessfulLogin(ApplicationUser user, string password, IList<string> roles)
    {
        _userManager.FindByNameAsync(user.UserName!).Returns(Task.FromResult<ApplicationUser?>(user));
        _signInManager.CheckPasswordSignInAsync(user, password, true)
            .Returns(Task.FromResult(IdentitySignInResult.Success));
        _userManager.GetRolesAsync(user).Returns(Task.FromResult(roles));
    }
}
