using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

public enum Roles {admin, cotch, user}
public class Man
{
    public int manId { get; set; }

    [Display(Name = "full name")]
    public string? name { get; set; }

    [Required]
    [Display(Name = "user name")]
    public string? userName { get; set; }

    [EmailAddress]
    [Required]
    [Display(Name ="Email address")]
    public string? email { get; set; }

    [Required]
    [NotMapped]
    [Display(Name = "Password")]
    public string? password { get; set; }

    [Required]
    [NotMapped]
    [Display(Name = "Compare Password")]
    [Compare("password")]
    public string? compare_password { get; set; }

    public string? passwordHash { get; set; }
    public int boy { get; set; }
    public int wight { get; set; }

    [Required]
    [Range(10, 100, ErrorMessage = "age must be between 10 and 100")]
    public int age { get; set; }

    [Display(Name = "Phone Number")]
    [Range(5000000000,5999999999 , ErrorMessage ="Phone number must be in this format: 5123456789")]
    public long number { get; set; }
    public Roles whoIam { get; set; }
    public List<Notification>? notifications;

}