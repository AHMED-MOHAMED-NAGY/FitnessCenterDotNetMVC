class Man
{
    int manId { get; set; }
    string name { get; set; }
    string userName { get; set; }
    string password { get; set; }
    int boy { get; set; }
    int wight { get; set; }
    int age { get; set; }
    string whoIam {get; set; }
    public List<Notification>? notifications;

    public Man(string name, string userName, string password, int boy, int wight, int age, string whoIam)
    {
        this.name = name;
        this.userName = userName;
        this.password = password;
        this.boy = boy;
        this.wight = wight;
        this.age = age;
        this.whoIam = whoIam;
    }
}