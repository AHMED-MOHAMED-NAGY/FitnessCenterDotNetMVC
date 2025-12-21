using System.Collections.Generic;

namespace fitnessCenter.Models
{
    public class AdminApplicationsViewModel
    {
        public List<User> NewUsers { get; set; }
        public List<Appointment> AppointmentHistory { get; set; }
    }
}
