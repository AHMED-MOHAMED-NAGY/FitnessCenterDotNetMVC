using System.ComponentModel.DataAnnotations;

public class Cotch : Man
{
    public string cotch_status { get; set; } // is work, not, free or available ...
    // available times
    public List <string> available_times { get; set; }
    public List<User> users_that_i_train { get; set; }


    public void add_user(User u)
    {
        users_that_i_train.Add(u);
    }
}