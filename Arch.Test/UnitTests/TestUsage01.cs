
using UsageApi.Domain;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Arch.Test.UnitTests
{
    public class TestUsage01
    {
        [Fact]
        public async Task Create_Test()
        {
            string title = "Test";
            string title2 = "Test2";
            var usage=new Usage01(title,Usage01Type.Type01);
            Assert.AreEqual(title, usage.Title);
            Assert.AreEqual(Usage01Type.Type01, usage.Type);
            usage.UpdateTitle(title2);
            Assert.AreEqual(title2, usage.Title);
            Assert.AreNotEqual(title, usage.Title);
        }
    }
}
