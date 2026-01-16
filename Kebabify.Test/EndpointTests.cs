using Kebabify.Api;
using Kebabify.Api.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using Shouldly;

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
            var result = await sut.MakeKebab(TestUtils.CreateMockRequest(request).Object);

            // assert
            var okResult = result.ShouldBeOfType<OkObjectResult>();
            var response = okResult.Value.ShouldBeOfType<KebabRespone>();

            response.Input.ShouldBe(input);
            response.Result.ShouldBe(expected);

            sut.KebabService.Verify(s => s.Create(input), Times.Once);
            sut.StorageService.Verify(s => s.Persist(input, expected), Times.Once);
        }

        [Fact]
        public async Task MakeKebab_Body_Size_Over_The_Limit_Should_Return_Bad_Request()
        {
            // arrange
            var input = new String('x', 1024);

            var sut = Testable.Create();

            var request = new KebabRequest { Input = input };

            // act
            var result = await sut.MakeKebab(TestUtils.CreateMockRequest(request).Object);

            // assert
            var badResult = result.ShouldBeOfType<BadRequestObjectResult>();
            badResult.Value.ShouldBe("Request body over the limit");
        }

        [Fact]
        public async Task MakeKebab_Empty_Body_Should_Return_Error()
        {
            // arrange
            var sut = Testable.Create();

            // act
            var result = await sut.MakeKebab(TestUtils.CreateMockRequest<string?>(null).Object);

            // assert
            var badResult = result.ShouldBeOfType<BadRequestObjectResult>();
            badResult.Value.ShouldBe("Invalid JSON or empty input");
        }

        [Fact]
        public async Task MakeKebab_No_Input_Should_Return_Bad_Request()
        {
            // arrange
            var sut = Testable.Create();

            var request = new KebabRequest { Input = string.Empty };

            // act
            var result = await sut.MakeKebab(TestUtils.CreateMockRequest(request).Object);

            // assert
            var badResult = result.ShouldBeOfType<BadRequestObjectResult>();
            badResult.Value.ShouldBe("Invalid JSON or empty input");
        }

        [Fact]
        public async Task MakeKebab_Input_Is_Over_The_Limit_Should_Return_Bad_Request()
        {
            // arrange
            var input = new String('x', 513);

            var sut = Testable.Create();

            var request = new KebabRequest { Input = input };

            // act
            var result = await sut.MakeKebab(TestUtils.CreateMockRequest(request).Object);

            // assert
            var badResult = result.ShouldBeOfType<BadRequestObjectResult>();
            var errors = badResult.Value.ShouldBeAssignableTo<IEnumerable<string>>();
            errors.ShouldContain("The field Input must be a string with a minimum length of 2 and a maximum length of 512.");
        }

        [Fact]
        public async Task MakeKebab_Input_Is_Under_The_Limit_Should_Return_Bad_Request()
        {
            // arrange
            var input = "x";

            var sut = Testable.Create();

            var request = new KebabRequest { Input = input };

            // act
            var result = await sut.MakeKebab(TestUtils.CreateMockRequest(request).Object);

            // assert
            var badResult = result.ShouldBeOfType<BadRequestObjectResult>();
            var errors = badResult.Value.ShouldBeAssignableTo<IEnumerable<string>>();
            errors.ShouldContain("The field Input must be a string with a minimum length of 2 and a maximum length of 512.");
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
