using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using CollTex.Models;

namespace CollTex.Controllers
{
    public class CollTexController : BaseController
    {
        private ColltexEntities db = new ColltexEntities();
        ResourceManager rm = new ResourceManager("CollTex.Resources.Resource", typeof(Resources.Resource).Assembly);
        // GET: CollTex
        [HttpGet]
        public JsonResult Convert(string epc)
        {
            try
            {
                
                var falseEpc = db.DetailEPCs.SingleOrDefault(x => x.IdEPC == epc && x.Status == true);
                if (falseEpc != null)
                {
                    var b = db.EPCs.SingleOrDefault(x => x.IdEPC == epc);
                    falseEpc.Status = false;
                    db.SaveChanges();
                    return Json(new { code = 200, item = b.IdGoods }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 300,  }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Epc(string epc,string id)
        {
            try
            {
                var session = (User)Session["user"];
                var FX = session.FXconnect.Id;
                Dele.DeleteDetailEPCs(FX);
                var epcItem = new EPC();
                var epcCheck = db.EPCs.SingleOrDefault(x => x.IdEPC == epc);               
                if (epcCheck == null)
                {
                    epcItem.IdEPC = epc;
                    epcItem.IdGoods = id;
                    epcItem.Status = true;
                    db.EPCs.Add(epcItem);
                    db.SaveChanges();                   
                    return Json(new { code = 200,msg = rm.GetString("receivedEPCsuccessfully").ToString()+" !!!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 300, msg = epc }, JsonRequestBehavior.AllowGet);
                }                         
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("getEPCerror").ToString()+" " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}