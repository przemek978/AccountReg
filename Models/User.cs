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
    [StringLength(11, MinimumLength = 11)]
    public string Pesel { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Surname { get; set; } = null!;

    [Required]
    [MaxLength(256)]
    [EmailAddress(ErrorMessage ="Invalid email format")]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    [SwaggerSchema(Format = "password")]
    [JsonIgnore]
    public string Password { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    [NotMapped]
    [SwaggerSchema(Format = "password")]
    [JsonIgnore]
    public string RePassword { get; set; } = null!;

    [Required]
    [MaxLength(9)]
    public string Phone{ get; set; } = null!;

    [Range(0, 100)]
    public int? Age { get; set; }

    [RegularExpression(@"^\d+(\.\d{1,3})?$")]
    public decimal? AvgCon { get; set; }



}

