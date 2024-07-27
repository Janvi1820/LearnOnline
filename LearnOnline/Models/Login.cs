using System.ComponentModel.DataAnnotations;

namespace LearnOnline.Models
{
    public class Login
    {

        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}


