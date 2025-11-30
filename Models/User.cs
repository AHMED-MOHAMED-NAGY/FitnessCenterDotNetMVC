using Microsoft.Net.Http.Headers;

class User : Man
{
    int userId { get; set; }
    DailyGoal? dailyGoal { get; set; }
    Cotch? cotch; // it will be available after read from db available cotchs has the same exercise
    string? subscribeStatus { get; set; } // left 30 days - finish
    int leftDaySubscribe;
    Exercise? exercise { get; set; } // this will be some thing like dropdown list 
                                    // after exercise selected cotch dropdown list will be available


    // functions
    public User(string name, string userName, string password, int boy, int wight, int age, string cotchName)
        : base(name, userName, password, boy, wight, age, "user")
    {
        set_cotch(cotchName);
        set_exercise();
        subscribeStatus = "subscribe not found!!";
        leftDaySubscribe = 0;
        notifications = new List<Notification>();
    }
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