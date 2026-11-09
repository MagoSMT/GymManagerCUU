using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GymManager.Filters
{
    public class AdminFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(
            ActionExecutingContext context)
        {
            var administrador = context.HttpContext.Session
                .GetString("Administrador");

            if (string.IsNullOrEmpty(administrador))
            {
                context.Result = new RedirectToActionResult(
                    "Login",
                    "Account",
                    null
                );
            }

            base.OnActionExecuting(context);
        }
    }
}