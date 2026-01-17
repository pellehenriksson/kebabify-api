using Kebabify.Api.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kebabify.Test.Services
{
    public class KebabServiceTests
    {
        [Fact]
        public void Create_Input_Is_Null_Should_Return_Empty_String()
        {
            var service = Testable.Create();
            var result = service.Create(null!);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Create_Input_Is_Empty_String_Should_Return_Empty_String()
        {
            var service = Testable.Create();
            var result = service.Create(string.Empty);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Create_Input_Is_Spaces_Only_Should_Return_Empty_String()
        {
            var service = Testable.Create();
            var result = service.Create(" ");

            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData("Hej alla barn, jag heter Rulle!", "hej-alla-barn-jag-heter-rulle")]
        [InlineData("-", "")]
        [InlineData("   -    ", "")]
        [InlineData("A raised fist was used as a logo by the Industrial Workers of the World[3]", "a-raised-fist-was-used-as-a-logo-by-the-industrial-workers-of-the-world3")]
        [InlineData("djqwi 141 fewf uif23rm99 &/I(e 452675367", "djqwi-141-fewf-uif23rm99-ie-452675367")]
        [InlineData("  leading and trailing spaces  ", "leading-and-trailing-spaces")]
        [InlineData("multiple   spaces", "multiple-spaces")]
        [InlineData("UPPERCASE", "uppercase")]
        public void Create_Valid_Input_Should_Make_Kebab(string input, string expectedResult)
        {
            var service = Testable.Create();
            var result = service.Create(input);

            Assert.Equal(expectedResult, result);
        }

        public class Testable : KebabService
        {
            public Testable(ILogger<KebabService> logger) : base(logger)
            {
            }

            public static Testable Create()
            {
                return new Testable(new NullLogger<KebabService>());
            }
        }
    }
}
