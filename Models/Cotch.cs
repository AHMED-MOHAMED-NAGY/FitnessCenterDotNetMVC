using System.ComponentModel.DataAnnotations;

public class Cotch : Man
{
    public string cotch_status { get; set; } // is work, not, free or available ...
    // available times
    public List <string> available_times { get; set; }
    public List<User> users_that_i_train { get; set; }

    Cotch() { }
    public Cotch(string name, string userName, string password, int boy, int wight, int age)
        : base(name, userName, password, boy, wight, age, "cotch")
    {
        cotch_status = "free";
        available_times = new List<string>();
        users_that_i_train = new List<User>();
    }
    public void add_user(User u)
    {
        users_that_i_train.Add(u);
    }
}