using System.ComponentModel.DataAnnotations;

namespace LearnOnline.Models
{
    public class Coursethumb
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public IFormFile Thumbnail { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }

}
