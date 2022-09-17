using FluentAssertions;

namespace HBD.Web.Auth.Tests.Scenarios.WithAdApp;

public class JwtAuthTests : IClassFixture<JwtAuthFixture>
{
    private readonly JwtAuthFixture _fixture;

    public JwtAuthTests(JwtAuthFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task CallApiWithoutAuth()
    {
        var client = _fixture.Client;
        Func<Task> action = () => client.GetStringAsync("/WeatherForecast");

        await action.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task CallApiWithAuth()
    {
        //Ensure Web App Started
        var client = _fixture.Client;

        var token = await _fixture.GetAppToken();

        //Call with original token
        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
        var rs = await client.GetStringAsync("/WeatherForecast");
        rs.Should().NotBeNullOrWhiteSpace();

        //Modify token and call again
        var newToken = token.UpdateWith("ver", "1.1");

        //Call with modified token
        client.DefaultRequestHeaders.Remove("Authorization");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", newToken);
        Func<Task> action = () => client.GetStringAsync("/WeatherForecast");
        await action.Should().ThrowAsync<HttpRequestException>();
    }
}