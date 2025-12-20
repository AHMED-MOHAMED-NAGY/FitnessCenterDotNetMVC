using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace fitnessCenter.Attributes
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

            // 1. LOGIN CHECK: If not logged in, go to Login
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new RedirectToActionResult("Login", "Home", null);
                return;
            }

            // 2. ROLE CHECK
            if (!string.IsNullOrEmpty(_requiredRole))
            {
                // SCENARIO A: The user has the exact role required (e.g. User tries to enter User page) -> ALLOW
                bool matchExact = (userRole == _requiredRole);
                
                // SCENARIO B: The user is an Admin (Admin tries to enter User page) -> ALLOW
                bool isAdmin = (userRole == "Admin");

                // If NEITHER is true, block them.
                if (!matchExact && !isAdmin)
                {
                    // This blocks "User" from accessing [Role("Admin")]
                    context.Result = new RedirectToActionResult("Index", userRole, null);
                }
            }
            
            base.OnActionExecuting(context);
        }
    }
}