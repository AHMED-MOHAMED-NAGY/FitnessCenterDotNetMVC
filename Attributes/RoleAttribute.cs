using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace fitnessCenter.Attributes // <--- Check this namespace
{
    public class RoleAttribute : ActionFilterAttribute
    {
        private readonly string _requiredRole;

        public RoleAttribute()
        {
        }

        public RoleAttribute(string role)
        {
            _requiredRole = role;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userId = session.GetInt32("UserId").ToString();
            var userRole = session.GetString("Role");

            // 1. Check if user is logged in (Session exists)
            if (string.IsNullOrEmpty(userId))
            {
                // Redirect to Login if session is missing
                context.Result = new RedirectToActionResult("Login", "Home", null);
                return;
            }

            // 2. Check if role matches (only if a role was passed)
            if (!string.IsNullOrEmpty(_requiredRole))
            {
                if (userRole != _requiredRole)
                {
                    // Redirect to Access Denied or Home if role is wrong
                    context.Result = new RedirectToActionResult("Index", "Home", null);
                }
            }

            base.OnActionExecuting(context);
        }
    }
}