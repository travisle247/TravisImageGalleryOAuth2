using Microsoft.EntityFrameworkCore;


namespace TravisImageGallery.API.Entities
{
    public class GalleryContext : DbContext
    {
        public GalleryContext(DbContextOptions<GalleryContext> options)
           : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Image> Images { get; set; }
    }
}
