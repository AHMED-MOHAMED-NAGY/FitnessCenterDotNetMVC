using System.Runtime.InteropServices;

class Notification
{
    int notId { get; set; }
    string title { get; set; }
    string msj { get; set; }
    string date { get; set; }

    public Notification(string title, string msj)
    {
        this.title = title;
        this.msj = msj;
        date = DateTime.Now.ToString("M/d/yyyy");
    }
}