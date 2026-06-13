using System.Net;
using System.Text;

namespace CSharpApp.Tests;

public class CommonServices
{

    public class HttpMessageHandlerStub : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public HttpMessageHandlerStub(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }

    public static HttpResponseMessage CreateHttpResponse(string json, HttpStatusCode statusCode) //Fake HTTP response
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json")
        };
    }
}