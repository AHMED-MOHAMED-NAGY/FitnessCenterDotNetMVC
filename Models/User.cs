using Microsoft.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

public class User : Man
{
    public DailyGoal? dailyGoal { get; set; }
    
    public int? CotchId { get; set; }
    [ForeignKey("CotchId")]
    public Cotch? cotch { get; set; } // it will be available after read from db available cotchs has the same exercise

    [Display(Name = "Subscribe Status")]
    public string? subscribeStatus { get; set; } // "New", "PendingPayment", "Active", "Expired"
    
    public string? SubscriptionPlan { get; set; } // "Weekly", "Monthly"
    public DateTime? SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }

    public int leftDaySubscribe;
    public Exercise? exercise { get; set; } // this will be some thing like dropdown list 
                                            // after exercise selected cotch dropdown list will be available


    // functions

    public void set_cotch(string cotchName)
    {
        // search in DB and set the cotch
        cotch = null; ////////////////////////////
        if (cotch != null)
            cotch.add_user(this);
    }
    public void set_exercise()
    {
        // set from user page
        exercise = null;
    }
    public void set_daily_goal(DailyGoal dg)
    {
        dailyGoal = dg;
    }
    public void update_subscribe_status(string s)
    {
        subscribeStatus = s;
    }
    public void add_notification(Notification n)
    {
        if(notifications != null)
            notifications.Add(n);
    }
    public void update_left_subscribe_days()
    {
        if(leftDaySubscribe != 0)
        {
            leftDaySubscribe--;
        }
        else
        {
            update_subscribe_status("your subscribe is finish!!");
        }
    }
    void list_notifications()
    {
        // list notifications from DB
    }
    // send complaint to admin
    // sned notification to cochs (chat with cotch)
    // make subscribe
    // change exercise
    // chat will ai
}