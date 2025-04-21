using System.Net;
using System.Net.Http.Json;

using AlphaDynamics.Poco;

using FluentAssertions;

namespace AlphaDynamics.WinForms.ApiClient.Tests;

public class ApiClientTests
{
    [Fact]
    public async Task GetCrewAsync_ShouldReturnCrewList()
    {
        // Arrange
        var expected = new List<Crew>
        {
            new() { Id = 1, Name = "Janet" },
            new() { Id = 2, Name = "Miles" }
        };

        var handler = new FakeHandler(HttpStatusCode.OK, JsonContent.Create(expected));
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:1234")
        };

        var api = new ApiClient(client);

        // Act
        var result = await api.GetCrewAsync();

        // Assert
        result.Should().BeEquivalentTo(expected);
        handler.LastRequest!.RequestUri!.AbsolutePath.Should().Be("/api/crew");
    }

    [Fact]
    public async Task GetCrewAsync_WhenServerError_ShouldReturnEmptyList()
    {
        // Arrange
        var handler = new FakeHandler(HttpStatusCode.InternalServerError, new StringContent(""));
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:1234")
        };

        var api = new ApiClient(client);

        // Act
        var result = await api.GetCrewAsync();

        // Assert
        result.Should().BeEmpty();
        handler.LastRequest!.RequestUri!.AbsolutePath.Should().Be("/api/crew");
    }

    private class FakeHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;
        public HttpRequestMessage? LastRequest { get; private set; }

        public FakeHandler(HttpStatusCode statusCode, HttpContent content)
        {
            _response = new HttpResponseMessage(statusCode)
            {
                Content = content
            };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(_response);
        }
    }
}
