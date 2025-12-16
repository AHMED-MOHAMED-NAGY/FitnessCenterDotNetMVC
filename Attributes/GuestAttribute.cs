using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace fitnessCenter.Attributes
{
    // This attribute ensures ONLY guests (not logged-in users) can see the page
    public class GuestAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userId = session.GetInt32("UserId").ToString();

            // Check if user IS logged in
            if (!string.IsNullOrEmpty(userId))
            {
                // If they are logged in, kick them out of the Login page
                // Redirect them to their Dashboard or Home
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }

            base.OnActionExecuting(context);
        }
    }
}