using System.Threading;
using Application.Values;
using Xunit;

namespace Application.Tests.Values
{
    public class ValuesTest : TestBase
    {
        [Fact]
        public void Should_Return_SingleValue()
        {
            var context = GetDbContext();

            var query = new Details.Query
            {
                Id = 2
            };
            
            var sut = new Details.Handler(context);
            var result = sut.Handle(query, CancellationToken.None).Result;
            
            Assert.Equal("Value 102", result.Name);
        }
    }
}