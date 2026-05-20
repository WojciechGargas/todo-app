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
        // Arrange
        const string email = "owner@test.com";
        const string password = "Secret123!";

        // Act
        var response = await SignInAsync(email, password);
        var jwt = await response.Content.ReadFromJsonAsync<JwtDto>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(jwt);
        Assert.False(string.IsNullOrWhiteSpace(jwt.AccessToken));
    }

    [Fact]
    public async Task SignIn_WithInvalidCredentials_ReturnsBadRequestAndErrorPayload()
    {
        // Arrange
        const string email = "owner@test.com";
        const string password = "xxx";

        // Act
        var response = await SignInAsync(email, password);
        
        // Assert
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("invalid_credentials", error.Code);
        Assert.Equal("Invalid credentials", error.Reason);
    }

    [Fact]
    public async Task SignIn_BeforeEmailConfirmation_ReturnsBadRequestAndErrorPayload()
    {
        // Arrange
        var email = CreateUniqueEmail();
        const string password = "User123!";
        
        var signupResponse = await SignUpAsync(email, password);
        Assert.Equal(HttpStatusCode.Created, signupResponse.StatusCode);
        
        // Act
        var response = await SignInAsync(email, password);
        
        // Assert
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("email_not_confirmed", error.Code);
    }
    [Fact]
    public async Task SignIn_AfterEmailConfirmation_ReturnsOkAndJwt()
    {
        // Arrange
        var email = CreateUniqueEmail();
        const string password = "User123!";
        var (signUpResponse, confirmResponse) = await SignUpAndConfirmAsync(email, password);
        Assert.Equal(HttpStatusCode.Created, signUpResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, confirmResponse.StatusCode);

        // Act
        var response = await SignInAsync(email, password);
        var jwt = await response.Content.ReadFromJsonAsync<JwtDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(jwt);
        Assert.False(string.IsNullOrWhiteSpace(jwt.AccessToken));
    }

    [Fact]
    public async Task SignUp_WithValidData_ReturnsCreated_AndSendsConfirmationEmail()
    {
        // Arrange
        var email = CreateUniqueEmail();

        // Act
        var signupResponse = await SignUpAsync(email);

        // Assert
        Assert.Equal(HttpStatusCode.Created, signupResponse.StatusCode);
        Assert.Contains(factory.TestEmailConfirmationService.RegistrationEmails,
            x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SignUp_WithDuplicateUsername_ReturnsBadRequestAndErrorPayload()
    {
        //Arrange 
        const string takenUsername = "owner_user"; // seeded user in ApplicationWebFactory
        
        //Act
        var response = await SignUpAsync(username: takenUsername);
        
        //Assert
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("username_already_in_use", error.Code);
    }
    
    [Fact]
    public async Task SignUp_WithDuplicateEmail_ReturnsBadRequestAndErrorPayload()
    {
        //Arrange 
        const string takenEmail = "owner@test.com"; // seeded user in ApplicationWebFactory
       
        //Act
        var response = await SignUpAsync(email: takenEmail);
        
        //Assert
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("email_already_in_use", error.Code);
    }

    [Fact]
    public async Task ConfirmEmail_WithValidToken_ReturnsNoContent()
    {
        // Arrange
        var email = CreateUniqueEmail();
        var signupResponse = await SignUpAsync(email);
        Assert.Equal(HttpStatusCode.Created, signupResponse.StatusCode);
        var token = GetRegistrationToken(email);

        // Act
        var response = await ConfirmEmailAsync(token);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private Task<HttpResponseMessage> SignUpAsync(
        string? email = null,
        string? username = null,
        string password = "User123!",
        string fullName = "Signup Test FullName",
        string? role = null)
    {
        email ??= CreateUniqueEmail();
        username ??= $"signup_{Guid.NewGuid():N}".Substring(0, 20);

        var request = new
        {
            Email = email,
            Username = username,
            Password = password,
            FullName = fullName,
            Role = role
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
    
    private async Task<(HttpResponseMessage SignUpResponse, HttpResponseMessage ConfirmResponse)>
        SignUpAndConfirmAsync(string email, string password = "User123!")
    {
        var signUpResponse = await SignUpAsync(email, password);
        var token = GetRegistrationToken(email);
        var confirmResponse = await ConfirmEmailAsync(token);

        return (signUpResponse, confirmResponse);
    }
}
