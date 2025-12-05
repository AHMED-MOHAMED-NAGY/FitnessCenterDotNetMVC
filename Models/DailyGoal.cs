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


    //public int UserId { get; set; }
    //public User User { get; set; }


    // for database
    DailyGoal() 
    {
        //status = false;
        //goal = null;
    }
    DailyGoal (string? goal)
    {
        this.goal = goal;
        status = false;
    }
}