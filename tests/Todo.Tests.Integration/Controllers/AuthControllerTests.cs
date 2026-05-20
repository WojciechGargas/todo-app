using System.Net;
using System.Net.Http.Json;
using Todo.Application.DTO;
using Todo.Tests.Integration.Infrastructure;
using Todo.Tests.Integration.Shared;

namespace Todo.Tests.Integration.Controllers;

public class AuthControllerTests(ApplicationWebFactory factory) : IClassFixture<ApplicationWebFactory>, IAsyncLifetime
{
    private readonly HttpClient _backend = factory.CreateClient();
    
    public Task InitializeAsync() => factory.ResetStateAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task SignIn_WithValidCredentials_ReturnsJwtToken()
    {
        //Act
        var response = await SignInAsync("owner@test.com", "Secret123!");
        var jwt = await response.Content.ReadFromJsonAsync<JwtDto>();
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(jwt);
        Assert.False(string.IsNullOrWhiteSpace(jwt.AccessToken));
    }

    [Fact]
    public async Task SignIn_WithInvalidCredentials_ReturnsBadRequestAndErrorPayload()
    {
        //Act
        var response = await SignInAsync("owner@test.com", "xxx");
        
        //Assert
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("invalid_credentials", error.Code);
        Assert.Equal("Invalid credentials", error.Reason);
    }

    [Fact]
    public async Task SignUp_WithValidData_ReturnsCreated_AndSendsConfirmationEmail()
    {
        var email = CreateUniqueEmail();
        var signupResponse = await SignUpAsync(email);

        Assert.Equal(HttpStatusCode.Created, signupResponse.StatusCode);
        Assert.NotEmpty(factory.TestEmailConfirmationService.RegistrationEmails);
    }

    [Fact]
    public async Task ConfirmEmail_WithValidToken_ReturnsNoContent()
    {
        var email = CreateUniqueEmail();

        var signupResponse = await SignUpAsync(email);
        Assert.Equal(HttpStatusCode.Created, signupResponse.StatusCode);

        var token = GetRegistrationToken(email);
        var confirmResponse = await ConfirmEmailAsync(token);

        Assert.Equal(HttpStatusCode.NoContent, confirmResponse.StatusCode);
    }

    [Fact]
    public async Task SignIn_AfterEmailConfirmation_ReturnsOkAndJwt()
    {
        var email = CreateUniqueEmail();
        const string password = "User123!";

        var signupResponse = await SignUpAsync(email, password);
        Assert.Equal(HttpStatusCode.Created, signupResponse.StatusCode);

        var token = GetRegistrationToken(email);
        var confirmResponse = await ConfirmEmailAsync(token);
        Assert.Equal(HttpStatusCode.NoContent, confirmResponse.StatusCode);

        var signinResponse = await SignInAsync(email, password);
        var jwt = await signinResponse.Content.ReadFromJsonAsync<JwtDto>();

        Assert.Equal(HttpStatusCode.OK, signinResponse.StatusCode);
        Assert.NotNull(jwt);
        Assert.False(string.IsNullOrWhiteSpace(jwt.AccessToken));
    }

    private Task<HttpResponseMessage> SignUpAsync(
        string email,
        string password = "User123!",
        string? username = null,
        string fullName = "Signup Test FullName")
    {
        username ??= $"signup_{Guid.NewGuid():N}".Substring(0, 20);

        var request = new
        {
            Email = email,
            Username = username,
            Password = password,
            FullName = fullName
        };

        return _backend.PostAsJsonAsync("/auth/sign-up", request);
    }

    private string GetRegistrationToken(string email) =>
        factory.TestEmailConfirmationService.RegistrationEmails
            .Last(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
            .Token;

    private static string CreateUniqueEmail() =>
        $"signuptest-{Guid.NewGuid():N}@test.com";

    private Task<HttpResponseMessage> ConfirmEmailAsync(string token) =>
        _backend.PostAsJsonAsync("/auth/confirm-email", new { token });

    private Task<HttpResponseMessage> SignInAsync(string email, string password) =>
        _backend.PostAsJsonAsync("/auth/sign-in", new { email, password });
}
