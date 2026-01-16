using Kebabify.Api;
using Kebabify.Api.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using Shouldly;

using System.Text;
using System.Text.Json;

namespace Kebabify.Test
{
    public class EndpointTests
    {
        [Fact]
        public async Task MakeKebab_Input_Is_Valid_Should_Return_Result()
        {
            // arrange
            var input = "x y";
            var expected = "x-y";

            var sut = Testable.Create();

            var request = new KebabRequest { Input = input };

            sut.KebabService
                .Setup(x => x.Create(request.Input))
                .Returns(expected);

            sut.StorageService
                .Setup(x => x.Persist(request.Input, expected))
                .Returns(Task.CompletedTask);

            // act
            var result = await sut.MakeKebab(CreateMockRequest(request).Object);

            // assert
            var okResult = result.ShouldBeOfType<OkObjectResult>();
            var response = okResult.Value.ShouldBeOfType<KebabRespone>();

            response.Input.ShouldBe(input);
            response.Result.ShouldBe(expected);

            sut.KebabService.Verify(s => s.Create(input), Times.Once);
            sut.StorageService.Verify(s => s.Persist(input, expected), Times.Once);
        }

        private Mock<HttpRequestData> CreateMockRequest(KebabRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            var mockContext = new Mock<FunctionContext>();

            var mockRequest = new Mock<HttpRequestData>(mockContext.Object);
            mockRequest.Setup(x => x.Body).Returns(bodyStream);

            return mockRequest;
        }

        public class Testable(Mock<IKebabService> kebabService, Mock<IStorageService> storageService, ILogger<Endpoints> logger)
            : Endpoints(kebabService.Object, storageService.Object, logger)
        {
            public Mock<IKebabService> KebabService { get; } = kebabService;

            public Mock<IStorageService> StorageService { get; } = storageService;

            public static Testable Create()
            {
                return new Testable(new Mock<IKebabService>(), new Mock<IStorageService>(), new NullLogger<Endpoints>());
            }
        }
    }
}
