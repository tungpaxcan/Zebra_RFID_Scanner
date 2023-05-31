using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Zebra_RFID_Scanner.Models;

namespace Zebra_RFID_Scanner.Controllers
{
    public class RFIDController : Controller
    {
        private Entities db = new Entities();
        ResourceManager rm = new ResourceManager("Zebra_RFID_Scanner.Resources.Resource", typeof(Resources.Resource).Assembly);
        // GET: RFID
        //------------------RFID---------------
        public ActionResult Index()
        {
           
            return View();
        }
        [HttpGet]
        public JsonResult AllShowEPC()
        {
            try
            {
                var session = (User)Session["user"];
                var FX = session.FXconnect.Id;
                var a = (from b in db.DetailEpcs.Where(x => x.Status == true && x.IdFX == FX)
                         select new
                         {
                             epc = b.IdEPC,
                         }).ToList();
                return Json(new { code = 200, a = a }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }   
        [HttpPost]
        public JsonResult falseEPC(string epc)
        {
            try
            {
                var deepc = db.DetailEpcs.Find(epc);
                    deepc.Status = false;
                db.SaveChanges();
                return Json(new { code = 200, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public string Post(Root[] root)
        {
            foreach (var tag in root)
            {
                DetailEpc t = new DetailEpc
                {
                    IdEPC = tag.data.idHex,
                    IdFX = tag.data.userDefined,
                    Status = true,
                };

                if (!db.DetailEpcs.Any(x => x.IdEPC.Equals(tag.data.idHex)))
                    db.DetailEpcs.Add(t);
                db.SaveChanges();
            }

            return "";
        }


    }

}