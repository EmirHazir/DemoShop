using DemoShop.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DemoShop
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //Roller 
        protected void Application_AuthenticateRequest()
        {
            if (User == null) { return; }

            string userName = Context.User.Identity.Name;

            string[] roles = null;

            using (DContext db = new DContext())
            {
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == userName);

                roles = db.UserRoles.Where(x => x.UserId == dto.Id)
                        .Select(x => x.Role.RoleName).ToArray();
            }

            IIdentity userIdentity = new GenericIdentity(userName);

            IPrincipal newUserObj = new GenericPrincipal(userIdentity, roles);

            Context.User = newUserObj;    
        }
    }
}
