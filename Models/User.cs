using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountReg.Models;

public partial class User
{
    [Key]
    [Required]
    [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "PESEL number should consist of 11 digits.")]
    public string Pesel { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[A-Za-ząćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$", ErrorMessage = "Name can only contain uppercase and lowercase letters")]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[A-Za-ząćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$", ErrorMessage = "Surname can only contain uppercase and lowercase letters")]
    public string Surname { get; set; } = null!;

    [Required]
    [MaxLength(256)]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(32, MinimumLength = 8, ErrorMessage = "Password must be a minimum of 8 characters and a maximum of 32 characters")]
    [SwaggerSchema(Format = "password")]
    [JsonIgnore]
    public string Password { get; set; } = null!;

    [Required]
    [NotMapped]
    [SwaggerSchema(Format = "password")]
    [JsonIgnore]
    public string RePassword { get; set; } = null!;

    [Required]
    [RegularExpression(@"^[0-9]{9}$", ErrorMessage = "Phone number should consist of 9 digits")]
    public string Phone { get; set; } = null!;

    [Range(0, 100)]
    public int? Age { get; set; }

    [RegularExpression(@"^\d{1,9}(\,\d{1,3})?$", ErrorMessage = "Average consumption from the last 3 months should be rounded to 3 decimal places")]
    public decimal? AvgCon { get; set; }

    public bool ValidatePesel()
    {
        int[] multipliers = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };

        int sum = 0;
        for (int i = 0; i < multipliers.Length; i++)
        {
            sum += multipliers[i] * int.Parse(Pesel[i].ToString());
        }

        int control = 10 - sum % 10;

        if (control.ToString().Equals(Pesel[10].ToString()))
        {
            return true;
        }
        return false;
    }

}

