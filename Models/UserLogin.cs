using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
namespace AccountReg.Models
{
    public class UserLogin
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [SwaggerSchema(Format = "password")]
        public string Password { get; set; }
    }
}
