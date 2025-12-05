public class Man
{
    public int manId { get; set; }
    public string name { get; set; }
    public string userName { get; set; }
    public string password { get; set; }
    public int boy { get; set; }
    public int wight { get; set; }
    public int age { get; set; }
    public string whoIam {get; set; }
    public List<Notification>? notifications;

    public Man() { }
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