using System.ComponentModel.DataAnnotations;

namespace LearnOnline.Models
{
    public class Upload
    {

        [Key]
        public int Id { get; set; }
        [Required]
        public string Courses { get; set; }

        [Required]
        public string TopicName { get; set; } 

        public string VideoFile { get; set; }

        [Required]
        public string YouTubeLink { get; set; }
    }
}
