using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Users
{
    public class Create
    {
        public class UserData
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class UserDataValidator : AbstractValidator<UserData>
        {
            public UserDataValidator()
            {
                RuleFor(x => x.Username).NotNull().NotEmpty();
                RuleFor(x => x.Email).NotNull().EmailAddress().NotEmpty();
                RuleFor(x => x.Password).NotNull().NotEmpty();
            }
        }

        public class Command : IRequest<User>
        {
            public UserData User { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.User).NotNull().SetValidator(new UserDataValidator());
            }
        }

        public class Handler : IRequestHandler<Command, User>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly DataContext _context;
            private readonly IJwtGenerator _jwtGenerator;

            public Handler(UserManager<AppUser> userManager, DataContext context, IJwtGenerator jwtGenerator)
            {
                _userManager = userManager;
                _context = context;
                _jwtGenerator = jwtGenerator;
            }
            
            public async Task<User> Handle(Command request, CancellationToken cancellationToken)
            {
                // check to see if username is in use
                if (await _context.Users.Where(x => x.UserName == request.User.Username).AnyAsync(cancellationToken))
                {
                    throw new RestException(HttpStatusCode.BadRequest, new {Username = "In Use"});
                }
                
                // check to see if email is in use
                if (await _context.Users.Where(x => x.Email == request.User.Email).AnyAsync(cancellationToken))
                {
                    throw new RestException(HttpStatusCode.BadRequest, new {Email = "In Use"});
                }

                var user = new AppUser
                {
                    Email = request.User.Email,
                    UserName = request.User.Username
                };

                var result = await _userManager.CreateAsync(user, request.User.Password);

                if (result.Succeeded)
                {
                    return new User
                    {
                        Username = user.UserName,
                        Email = user.Email,
                        Token = _jwtGenerator.CreateToken(user)
                    };
                }
                
                throw new Exception("Something went wrong");
            }
        }
    }
}