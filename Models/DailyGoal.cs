using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

//[Keyless]
public class DailyGoal
{
    [Key]
    public int goalId { get; set; }
    public bool status { get; set; } // is done or not ...
    public string? goal { get; set; }
    public string date { get; set; }


    public int? UserId { get; set; }
    public User User { get; set; }


    // for database
    public DailyGoal() 
    {
        status = false;
        date = DateTime.Now.ToString("yyyy-MM-dd");
    }

    public DailyGoal(string goal)
    {
        this.goal = goal;
        status = false;
        date = DateTime.Now.ToString("yyyy-MM-dd");
    }
}