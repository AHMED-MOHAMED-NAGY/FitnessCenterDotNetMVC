using System.ComponentModel.DataAnnotations;

public class Exercise
{
    [Key]
    public int exId { get; set; }
    public string? exerciseType { get; set; }
    [Range(0, 100000)]
    public decimal Price { get; set; }
    public List<Cotch>? availCotchs { get; set; }
}