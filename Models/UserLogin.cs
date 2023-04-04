using System.ComponentModel.DataAnnotations;
namespace AccountReg.Models
{
    public class UserLogin
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
