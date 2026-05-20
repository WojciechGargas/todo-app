using System.Net;
using System.Net.Http.Json;
using Todo.Application.DTO;
using Todo.Tests.Integration.Infrastructure;

namespace Todo.Tests.Integration.Controllers;

public class AuthControllerTests(ApplicationWebFactory factory) : IClassFixture<ApplicationWebFactory>, IAsyncLifetime
{
    private readonly HttpClient _backend = factory.CreateClient();
    
    public Task InitializeAsync() => factory.ResetStateAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task SignIn_WithValidCredentials_ReturnsJwtToken()
    {
        //Arrange
        var request = new 
        {
            email = "owner@test.com",
            password = "Secret123!"
        };
        
        //Act
        var response = await _backend.PostAsJsonAsync("/auth/sign-in", request);
        var jwt = await response.Content.ReadFromJsonAsync<JwtDto>();
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(jwt);
        Assert.False(string.IsNullOrWhiteSpace(jwt.AccessToken));
    }
}