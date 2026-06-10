using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace manage_my_hairsaloon.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
    }
}
