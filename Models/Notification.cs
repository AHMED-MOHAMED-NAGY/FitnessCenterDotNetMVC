using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

public class Notification
{
    [Key]
    public int notId { get; set; }
    public string title { get; set; }
    public string msj { get; set; }
    public string date { get; set; }
    public bool IsRead { get; set; } = false;
    
    public int ManId { get; set; }
    [ForeignKey("ManId")]
    public Man Man { get; set; }

    Notification() { }

    public Notification(string title, string msj)
    {
        this.title = title;
        this.msj = msj;
        date = DateTime.Now.ToString("M/d/yyyy");
    }
}