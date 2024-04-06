using FHP_Application;
using FHP_Web_UI.App_Start;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace FHP_Web_UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            cls_Init obj_ini = new cls_Init();

            obj_ini.Intialize();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

    
    }
}
