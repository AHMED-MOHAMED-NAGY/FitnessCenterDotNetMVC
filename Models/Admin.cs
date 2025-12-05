using System.ComponentModel.DataAnnotations;

public class Admin : Man
{

    // functions 
    Admin() { }
    public Admin(string name, string userName, string password, int boy, int wight, int age)
        : base(name, userName, password, boy, wight, age, "admin")
    {
        
    }
    // list users
    public void make_user_cotch(User u)
    {
        Cotch c = new Cotch(u.name, u.userName, u.password, u.boy, u.wight, u.age);
        delete_user(u); // deleting user after make is as cotch
    }
    public void make_user_admin(User u)
    {
        Admin a = new Admin(u.name, u.userName, u.password, u.boy, u.wight, u.age);
        delete_user(u);
    }
    public void make_cotch_admin(Cotch u)
    {
        Admin a = new Admin(u.name, u.userName, u.password, u.boy, u.wight, u.age);
        delete_cotch(u);
    }

    // delete user from db
    void delete_user(User u){}
    void delete_cotch(Cotch c){}
    // delete admin form db
    // list cotch
    // list ...
    // send discound
}