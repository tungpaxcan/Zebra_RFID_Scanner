using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zebra_RFID_Scanner.Models;

namespace Zebra_RFID_Scanner.Controllers
{
    public class HODController : BaseController
    {
        private Entities db = new Entities();
        // GET: HOD
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult DeleteCtn(int id,string idName)
        {
            try
            {
            
                var ctn = db.DataScanPhysicals.Find(id);
                db.DataScanPhysicals.Remove(ctn);
                db.SaveChanges();
                var Ctn = (from a in db.DataScanPhysicals.Where(x => x.IdReports == idName)
                           select new
                           {
                               so = a.So,
                               po = a.Po,
                               sku = a.Sku,
                               id = a.Id,
                               ctn = a.CartonTo,
                               status = a.Status == true ? "Matched" : "Mismatched",
                               upc = a.UPC,
                               qty = a.Qty
                           }).ToList();
                return Json(new { code = 200, Ctn = Ctn ,}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg ="false" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}