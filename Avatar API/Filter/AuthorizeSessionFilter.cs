using Avatar_API.Session;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OpenBound_Network_Object_Library.Session;
using SharpDX.X3DAudio;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Avatar_API.Filter
{

    public class AuthorizeSessionAttribute : TypeFilterAttribute
    {
        public AuthorizeSessionAttribute()
                             : base(typeof(AuthorizeSessionFilter))
        {
        }

        private class AuthorizeSessionFilter : IActionFilter
        {
            private readonly SessionHandler _sessionHandler;
            public AuthorizeSessionFilter(SessionHandler sessionHandler)
            {
                _sessionHandler = sessionHandler;
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                var controller = (Controller)context.Controller;
                string controllerName = context.Controller.GetType().Name;

                if(controllerName == "LoginController" && _sessionHandler.IsAuthenticated())
                {
                    context.Result = controller.RedirectToAction("Index", "Donation");
                }
                else if (!_sessionHandler.IsAuthenticated() && controllerName != "LoginController")
                {
                   context.Result = controller.RedirectToAction("Index", "Login");
                };
            }

            public void OnActionExecuted(ActionExecutedContext context) {}
        }
    }
}
