using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

namespace AccountReg.Models
{
    public class UserLogin
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [JsonIgnore]
        [SwaggerSchema(Format = "password")]
        public string Password { get; set; }
    }
}
