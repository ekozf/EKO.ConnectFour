﻿using System.ComponentModel.DataAnnotations;

namespace EKO.ConnectFour.Api.Models;

public class LoginModel
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [MinLength(6)]
    public required string Password { get; set; }
}