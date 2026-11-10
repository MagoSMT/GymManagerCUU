
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GymManager.Filters
{
    // Hereda de ActionFilterAttribute para controlar el acceso a las acciones
    public class AdminFilter : ActionFilterAttribute
    {
        // Verifica que exista una sesion de administrador
        public override void OnActionExecuting(
            ActionExecutingContext context)
        {
            var administrador = context.HttpContext.Session
                .GetString("Administrador");

            if (string.IsNullOrEmpty(administrador))
            {
                // Si no hay sesion regresa al inicio de sesion
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
