using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

using Moq;

using System.Text;
using System.Text.Json;

namespace Kebabify.Test
{
    public static class TestUtils
    {
        public static Mock<HttpRequestData> CreateMockRequest<T>(T request)
        {
            var json = JsonSerializer.Serialize(request);
            var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            var mockContext = new Mock<FunctionContext>();

            var mockRequest = new Mock<HttpRequestData>(mockContext.Object);
            mockRequest.Setup(x => x.Body).Returns(bodyStream);

            return mockRequest;
        }
    }
}
