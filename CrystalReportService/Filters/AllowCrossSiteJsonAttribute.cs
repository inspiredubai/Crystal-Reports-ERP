using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace CrystalReportService.Filters
{    
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response != null)
            {
                actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
                actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Methods", "*");
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}