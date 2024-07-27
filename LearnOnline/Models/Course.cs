using System.ComponentModel.DataAnnotations;

namespace LearnOnline.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        public string Thumbnail { get; set; }
        public decimal Price { get; set; }
        [Required]
        public string Description { get; set; }

      
    }

}
