using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Application.Comments;
using Application.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Application.Tests.Comments
{
    public class DeleteTest : TestBase
    {
        [Fact]
        public async void Should_Delete_Comment()
        {
            var userAccessor = new Mock<IUserAccessor>();
            userAccessor.Setup(u => u.GetCurrentUsername()).Returns("test");
            
            var context = GetDbContext();
            
            var user = await context.Users.AddAsync(new AppUser
            {
                Id = 1,
                Email = "test@test.com",
                UserName = "test"
            });

            context.Activities.Add(new Activity {
                Id = 1, 
                Title = "Test Activity 1", 
                Date = DateTime.Now.AddDays(2),
                Comments = new List<Comment>
                {
                    new Comment
                    {
                        Id = 1,
                        Body = "test comment 1",
                        Author = user.Entity
                    },
                    new Comment
                    {
                        Id = 2,
                        Body = "test comment 2"
                    }
                }});
            await context.Activities.AddAsync(new Activity
                {Id = 2, Title = "Test Activity 2", Date = DateTime.Now.AddDays(1)});
            await context.SaveChangesAsync();
            
            var sut = new Delete.Handler(context, userAccessor.Object);
            var result = sut.Handle(new Delete.Command(1, 1), CancellationToken.None);

            var activity = await context.Activities.FirstOrDefaultAsync(x => x.Id == 1);
            
            Assert.Equal(1, activity.Comments.Count);
        }
    }
}