using TravisImageGallery.API.Entities;
using System;
using System.Collections.Generic;

namespace TravisImageGallery.API.Services
{
    public interface IGalleryRepository
    {
        IEnumerable<Image> GetImages();
        Image GetImage(Guid id);
        bool ImageExists(Guid id);
        void AddImage(Image image);
        void UpdateImage(Image image);
        void DeleteImage(Image image);
        bool Save();
    }
}
