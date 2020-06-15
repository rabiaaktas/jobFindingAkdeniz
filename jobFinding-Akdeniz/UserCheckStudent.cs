using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jobFinding_Akdeniz
{
    public class UserCheckStudent : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(LoginStatus.Current.IsLogin != true || LoginStatus.Current.UserType != 2)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                {
                    {"controller","Login" }, {"action","OgrenciLogin"}
                }); ;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}