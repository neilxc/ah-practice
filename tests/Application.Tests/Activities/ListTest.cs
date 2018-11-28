using System.Threading;
using Application.Activities;
using AutoMapper;
using Domain;
using Xunit;

namespace Application.Tests.Activities
{
    public class ListTest : TestBase
    {
        private readonly IMapper _mapper;

        public ListTest()
        {
            var mockMapper = new MapperConfiguration(cfg => {cfg.AddProfile(new MappingProfile());});
            _mapper = mockMapper.CreateMapper();
        }
        
        [Fact]
        public void Should_Return_List_Of_Two_Activities()
        {
            var context = GetDbContext();
            context.Activities.Add(new Activity {Id = 1, Title = "Test Activity 1"});
            context.Activities.Add(new Activity {Id = 2, Title = "Test Activity 2"});
            context.SaveChanges();

            var sut = new List.Handler(context);
            var result = sut.Handle(new List.Query(), CancellationToken.None).Result;
            
            Assert.Equal("Test Activity 1", result.Activities[0].Title);
            Assert.Equal("Test Activity 2", result.Activities[1].Title);
            Assert.Equal(2, result.ActivityCount);
        }

    }
}