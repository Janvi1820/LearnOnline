using System.ComponentModel.DataAnnotations;

namespace LearnOnline.Models
{
    public class Auth
    {
        
            [Key]
            public int Id { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }

            public string Role { get; set; } // Added Role property

    }
}
