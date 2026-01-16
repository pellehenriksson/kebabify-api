using Kebabify.Api.Common;

using Shouldly;

namespace Kebabify.Test.Common
{
    public class KebabifyExtensionsTests
    {
        [Fact]
        public void ToJson_Should_Serialize_String()
        {
            var item = new TestItem("Hello world");

            var result = item.ToJson();

            result.ShouldBe("{\"Message\":\"Hello world\"}");
        }

        public record TestItem(string Message);
    }
}
