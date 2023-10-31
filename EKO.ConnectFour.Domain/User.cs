using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EKO.ConnectFour.Domain;

public class User : IdentityUser<Guid>
{
    [Required]
    public required string NickName { get; set; }
}
