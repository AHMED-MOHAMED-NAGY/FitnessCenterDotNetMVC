using Microsoft.AspNetCore.Mvc;
using fitnessCenter.Models;
using System.Linq;

namespace fitnessCenter.ViewComponents
{
    public class NotificationBadgeViewComponent : ViewComponent
    {
        private readonly FitnessContext _context;

        public NotificationBadgeViewComponent(FitnessContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            int unreadCount = 0;

            if (userId != null)
            {
                unreadCount = _context.notifications
                    .Count(n => n.ManId == userId && !n.IsRead);
            }

            return View(unreadCount);
        }
    }
}
