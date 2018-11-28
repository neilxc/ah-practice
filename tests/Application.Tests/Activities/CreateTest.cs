using System;
using System.Threading;
using Application.Activities;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Moq;
using Xunit;

namespace Application.Tests.Activities
{
    public class CreateTest : TestBase
    {
        private readonly IMapper _mapper;
        public CreateTest()
        {
            var mockMapper = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            _mapper = mockMapper.CreateMapper();
        }

        [Fact]
        public void Should_Create_Activity()
        {
            var userAccessor = new Mock<IUserAccessor>();
            userAccessor.Setup(u => u.GetCurrentUsername()).Returns("test");
            
            var context = GetDbContext();

            context.Users.AddAsync(new AppUser
            {
                Id = 1,
                Email = "test@test.com",
                UserName = "test"
            });
            context.SaveChanges();

            var activityCommand = new Create.Command
            {
                Activity = new Create.ActivityData
                {
                    City = "London",
                    Date = DateTime.Today,
                    Description = "Description of event",
                    GeoCoordinate = new GeoCoordinate
                    {
                        Id = 1,
                        Latitude = 20,
                        Longitude = 40
                    },
                    Title = "Test Activity",
                    Venue = "Test Venue"
                }
            };
            
            var sut = new Create.Handler(context, userAccessor.Object, _mapper);
            var result = sut.Handle(activityCommand, CancellationToken.None).Result;
            
            Assert.NotNull(result);
            Assert.Equal("Test Activity", result.Title);
        }
    }
}