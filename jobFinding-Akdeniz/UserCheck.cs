using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jobFinding_Akdeniz
{
    public class UserCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (LoginStatus.Current.IsLogin != true)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                {
                    {"controller","Login" }, {"action","OgrenciGirisi"}
                }); ;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}