class Cotch : Man
{
    int cotchId;
    string cotch_status; // is work, not, free or available ...
    // available times
    List <string> available_times;
    List<User> users_that_i_train;

    public Cotch(string name, string userName, string password, int boy, int wight, int age, string cotchName)
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