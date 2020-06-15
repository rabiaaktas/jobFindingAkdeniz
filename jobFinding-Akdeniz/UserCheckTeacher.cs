using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jobFinding_Akdeniz
{
    public class UserCheckTeacher : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!LoginStatus.Current.IsLogin || LoginStatus.Current.UserType != 3)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                {
                    {"controller","Home" }, {"action","Login"}
                }); ;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}