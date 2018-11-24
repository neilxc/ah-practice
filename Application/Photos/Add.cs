using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Add
    {
        public class PhotoData
        {
            public IFormFile File { get; set; }
        }

        public class PhotoDataValidator : AbstractValidator<PhotoData>
        {
            public PhotoDataValidator()
            {
                RuleFor(x => x.File).NotNull();
            }
        }

        public class Command : IRequest<PhotoDto>
        {
            public PhotoData Photo { get; set; }
        }

        public class Handler : IRequestHandler<Command, PhotoDto>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IMapper _mapper;
            private readonly ICloudinaryAccessor _cloudinaryAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor, IMapper mapper, ICloudinaryAccessor cloudinaryAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
                _mapper = mapper;
                _cloudinaryAccessor = cloudinaryAccessor;
            }
            
            public async Task<PhotoDto> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.Include(p => p.Photos)
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentUsername(), cancellationToken);

                var photo = _cloudinaryAccessor.AddPhotoForUser(request.Photo.File);

                if (!user.Photos.Any(x => x.IsMain))
                    photo.IsMain = true;
                
                user.Photos.Add(photo);

                await _context.SaveChangesAsync(cancellationToken);

                var photoDto = _mapper.Map<Photo, PhotoDto>(photo);

                return photoDto;
            }
        }
    }
}