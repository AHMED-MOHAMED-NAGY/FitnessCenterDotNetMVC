using System.ComponentModel.DataAnnotations;

public class Exercise
{
    [Key]
    public int exId { get; set; }
    public string? exerciseType { get; set; }
    public List<Cotch>? availCotchs;
}