using DemoWeb.Areas.DOTNETGROUP.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DemoWeb.Areas.DOTNETGROUP.Controllers
{
    public class BaseController : Controller
    {
        // GET: DOTNETGROUP/Base
        protected override void OnActionExecuted(ActionExecutedContext filtercontext)
        {
            var session = (LoginModel)Session[Constants.USER_SESSION];
            if (session == null)
            {
                filtercontext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index", Ares = "DOTNETGROUP" }));
            }
            base.OnActionExecuted(filtercontext);
        }

        protected void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;
            switch (type)
            {
                case "success":
                    TempData["AlertType"] = "alert-success"; break;
                case "warning":
                    TempData["AlertType"] = "alert-warning"; break;
                case "error":
                    TempData["AlertType"] = "alert-error"; break;
                default: TempData["AlertType"] = ""; break;
            }
        }
    }
}