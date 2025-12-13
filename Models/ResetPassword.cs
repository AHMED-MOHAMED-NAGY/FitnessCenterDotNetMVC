using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fitnessCenter.Models
{
    public class ResetPassword
    {
        [Required]
        [Display(Name = "Password")]
        public string? password { get; set; }

        [Required]
        [Display(Name = "Compare Password")]
        [Compare("password")]
        public string? compare_password { get; set; }
        public string? Key { get; set; }
    }
}
