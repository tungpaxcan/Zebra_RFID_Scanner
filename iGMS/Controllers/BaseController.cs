using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using Zebra_RFID_Scanner.Models;

namespace Zebra_RFID_Scanner.Controllers
{
    public class BaseController : Controller
    {
        private Entities db = new Entities();
        // GET: Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["user"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new { controller = "Login", action = "Login" }));
            }
        }
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (Session["CurrentCulture"] != null)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Session["CurrentCulture"].ToString());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["CurrentCulture"].ToString());
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                Session["CurrentCulture"] = "en-US";
            }
        }
        [HttpGet]
        public JsonResult ChangCulture(string ddlCulture, string returnUrl)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(ddlCulture);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(ddlCulture);
                Session["CurrentCulture"] = ddlCulture;
                return Json(new { code = 200, url = returnUrl }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
           
        }
        [HttpGet]
        public JsonResult language()
        {
            try
            {
                var language = Session["CurrentCulture"];
                return Json(new { code = 200, language= language }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult SignOut()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                Session["user"]=null;
               return Json(new { code = 200, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}