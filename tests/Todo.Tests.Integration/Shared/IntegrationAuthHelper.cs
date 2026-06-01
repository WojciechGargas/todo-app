using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Todo.Application.DTO;

namespace Todo.Tests.Integration.Shared;

internal static class IntegrationAuthHelper
{
    public static Task<HttpResponseMessage> SignInAsync(HttpClient client, string email, string password)
        => client.PostAsJsonAsync("/auth/sign-in", new { email, password });

    public static void SetBearerToken(HttpClient client, string accessToken)
        => client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    public static void ClearBearerToken(HttpClient client)
        => client.DefaultRequestHeaders.Authorization = null;

    public static async Task<JwtDto> ReadJwtOrThrowAsync(HttpResponseMessage response)
    {
        var jwt = await response.Content.ReadFromJsonAsync<JwtDto>();
        if (jwt is null || string.IsNullOrWhiteSpace(jwt.AccessToken))
        {
            throw new InvalidOperationException("JWT token was not returned in sign-in response.");
        }

        return jwt;
    }
    
    public static async Task<string> SignInAndSetBearerTokenAsync(
        HttpClient client,
        string email,
        string password)
    {
        var signInResponse = await SignInAsync(client, email, password);
        if (signInResponse.StatusCode != HttpStatusCode.OK)
        {
            throw new InvalidOperationException($"Sign-in failed with status code: {signInResponse.StatusCode}");
        }

        var jwt = await ReadJwtOrThrowAsync(signInResponse);
        SetBearerToken(client, jwt.AccessToken);

        return jwt.AccessToken;
    }
}
