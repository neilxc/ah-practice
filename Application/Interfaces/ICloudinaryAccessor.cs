using Domain;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface ICloudinaryAccessor
    {
        Photo AddPhotoForUser(IFormFile file);
        string DeletePhotoForUser(Photo photo);
    }
}