using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AccountReg.Models;

public partial class User
{
    [Key]
    [Required]
    [StringLength(11)]
    public string Pesel { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Surname { get; set; } = null!;

    [Required]
    [MaxLength(256)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Password { get; set; } = null!;

    [Range(0, 150)]
    public int? Age { get; set; }

    [RegularExpression(@"^\d+(\.\d{1,3})?$")]
    public decimal? AvgCon { get; set; }
}

