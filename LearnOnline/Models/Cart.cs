using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnOnline.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public bool IsPurchased { get; set; }
       
        [ForeignKey("CourseId")]
        public Course Course { get; set; }

       
    }
}
