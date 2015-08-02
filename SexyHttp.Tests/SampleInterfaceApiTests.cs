using NUnit.Framework;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class SampleInterfaceApiTests
    {
        [Test]
        public void SimpleGetString()
        {
            var api = Http.Create<ISampleInterfaceApi>();

        }
    }
}