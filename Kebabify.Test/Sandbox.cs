using Xunit.Abstractions;

namespace Kebabify.Test
{
    public class Sandbox
    {
        private readonly ITestOutputHelper outputHelper;

        public Sandbox(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        [Fact]
        public void Run()
        {

            this.outputHelper.WriteLine(Guid.NewGuid().ToString());

            Assert.True(true);
        }
    }
}