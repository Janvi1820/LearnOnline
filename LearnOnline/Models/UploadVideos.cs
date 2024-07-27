using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace LearnOnline.Models
{
    public class UploadVideos
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Courses { get; set; }

        [Required]
        public string TopicName { get; set; }
        [AllowNull]
        public IFormFile VideoFile { get; set; }

        public string YouTubeLink { get; set; }
    }
}
