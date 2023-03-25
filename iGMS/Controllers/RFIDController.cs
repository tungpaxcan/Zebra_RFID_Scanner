using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using CollTex.Models;

namespace CollTex.Controllers
{
    public class RFIDController : Controller
    {
        private ColltexEntities db = new ColltexEntities();
        ResourceManager rm = new ResourceManager("CollTex.Resources.Resource", typeof(Resources.Resource).Assembly);
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
                var a = (from b in db.DetailEPCs.Where(x => x.Status==true&&x.IdFX==FX)
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
        [HttpGet]
        public JsonResult CompareEPC(string epc)
        {
            try
            {
                var a = (from b in db.DetailEPCs.Where(x => x.IdEPC == epc&&x.Status==true)
                         join bb in db.EPCs on b.IdEPC equals bb.IdEPC
                         join bbb in db.Goods on bb.IdGoods equals bbb.Id 
                         select new
                         {
                             idgood = bbb.IdGood
                         }).ToList();
                var c = db.DetailEPCs.SingleOrDefault(x => x.IdEPC == epc && x.Status == true);
                if (c != null)
                {
                    c.Status = false;
                    db.SaveChanges();
                }
                return Json(new { code = 200, a = a }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult CompareReceipt(string epc)
        {
            try
            {
                var a = (from b in db.DetailEPCs.Where(x => x.IdEPC == epc && x.Status == true)
                         join bb in db.EPCs on b.IdEPC equals bb.IdEPC
                         select new
                         {
                             idgood = bb.IdGoods
                         }).ToList();
                var c = db.DetailEPCs.SingleOrDefault(x => x.IdEPC == epc && x.Status == true);
                if (c != null)
                {
                    c.Status = false;
                    db.SaveChanges();
                }
                return Json(new { code = 200, a = a }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult StatusEPC(string epc)
        {
            try
            {
                var c = db.DetailEPCs.SingleOrDefault(x => x.IdEPC == epc && x.Status == true);
                if (c != null)
                {
                    c.Status = false;
                    db.SaveChanges();
                }
                else
                {

                }                   
                return Json(new { code = 200, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult DeleteEPC()
        {
            try
            {
                var session = (User)Session["user"];
                var FX = session.FXconnect.Id;
                Dele.DeleteDetailEPCs(FX);
                return Json(new { code = 200, msg = rm.GetString("success").ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false").ToString() + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public string Post(Root[] root)
        {
            foreach (var tag in root)
            {
                    DetailEPC t = new DetailEPC
                    {
                        IdEPC = tag.data.idHex,
                        IdFX = tag.data.userDefined,
                        Status = true,
                        Amount = 1,
                    };

                    if (!db.DetailEPCs.Any(x => x.IdEPC.Equals(tag.data.idHex)))
                        db.DetailEPCs.Add(t);
                db.SaveChanges();
            }
           
            return "";
        }

    }

}