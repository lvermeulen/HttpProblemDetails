using System.Xml.Linq;
using Newtonsoft.Json;
using Xunit;

namespace HttpProblemDetails.Tests
{
    public static class DeserializationHelper
    {
        public static void AssertFromJson(string s)
        {
            dynamic problemDetail = JsonConvert.DeserializeObject(s);
            Assert.NotNull(problemDetail);
            Assert.Equal("https://example.com/probs/out-of-credit", problemDetail.type.ToString());
            Assert.Equal("You do not have enough credit.", problemDetail.title.ToString());
            Assert.Equal("403", problemDetail.status.ToString());
            Assert.Equal("Your current balance is 30, but that costs 50.", problemDetail.detail.ToString());
            Assert.Equal("/account/12345/msgs/abc", problemDetail.instance.ToString());
        }

        public static void AssertFromXml(string s)
        {
            var startIndex = s.IndexOf('<');
            if (startIndex > 0)
            {
                s = s.Remove(0, startIndex);
            }
            var doc = XDocument.Parse(s);
            Assert.Equal("https://example.com/probs/out-of-credit", doc.Root?.Element("Type")?.Value);
            Assert.Equal("You do not have enough credit.", doc.Root?.Element("Title")?.Value);
            Assert.Equal("403", doc.Root?.Element("Status")?.Value);
            Assert.Equal("Your current balance is 30, but that costs 50.", doc.Root?.Element("Detail")?.Value);
            Assert.Equal("/account/12345/msgs/abc", doc.Root?.Element("Instance")?.Value);
        }
    }
}
