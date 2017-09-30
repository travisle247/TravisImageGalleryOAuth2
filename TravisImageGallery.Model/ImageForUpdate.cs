using System.ComponentModel.DataAnnotations;

namespace TraivsImageGallery.Model
{
    public class ImageForUpdate
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }      
    }
}
