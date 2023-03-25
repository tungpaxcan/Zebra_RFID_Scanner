using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using CollTex.Models;

namespace CollTex.Controllers
{
    public class HomeController : BaseController
    {
        private ColltexEntities db = new ColltexEntities();
        ResourceManager rm = new ResourceManager("CollTex.Resources.Resource", typeof(Resources.Resource).Assembly);
        public ActionResult Index()
        {

            return View();
        }
     

        [HttpGet]
        public JsonResult UserSession()
        {
            try
            {
                var c = (User)Session["user"];
                return Json(new { code = 200, name = c.Name,id=c.Id,admin=c.Admin}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false")+" " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult RefreshEPC()
        {
            try
            {
                var session = (User)Session["user"];
                var FX = session.FXconnect.Id;
                Dele.DeleteDetailEPCs(FX);
                return Json(new { code = 200, msg = rm.GetString("success").ToString()}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false").ToString()+" " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }    
    }
}