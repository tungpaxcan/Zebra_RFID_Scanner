using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CollTex.Models;
using System.Data.Entity.ModelConfiguration;
using System.Resources;

namespace CollTex.Controllers
{
    public class RegisterController : BaseController
    {
        private ColltexEntities db = new ColltexEntities();
        ResourceManager rm = new ResourceManager("CollTex.Resources.Resource", typeof(Resources.Resource).Assembly);
        // GET: Register
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Info()
        {
            var user = (User)Session["user"];
            var id = user.Id;
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User account = db.Users.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }
        public ActionResult ListUser()
        {
            return View();
        }
        [HttpGet]
        public JsonResult List(int pagenum,int page,string seach)
        {
            try
            {
                var pageSize = pagenum;
                var a = (from b in db.Users.Where(x => x.Id > 0)
                         select new
                         {
                             id = b.Id,
                             name = b.Name,
                             fx = b.FXconnect.Name,
                             admin=b.Admin
                         }).ToList().Where(x=>x.name.ToLower().Contains(seach)
                         ||x.name.Contains(seach));
                var pages = a.Count() % pageSize == 0 ? a.Count() / pageSize : a.Count() / pageSize + 1;
                var c = a.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var count = a.Count();
                return Json(new { code = 200,c = c,pages = pages,count =count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false").ToString()+ " !!! " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Add(string name, string userName, string password,string fx, string des, bool status, bool authorities)
        {
            try
            {
                User user = new User();
                user.Name = name;
                user.IdFX = fx;
                user.User1 = userName;
                user.Pass = Encode.ToMD5(password);
                user.Description = des;
                user.Status = status;
                user.Admin = authorities;
                db.Users.Add(user);
                db.SaveChanges();
                return Json(new { code = 200, msg =rm.GetString("accountsuccessfullycreated").ToString() +" !!!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("accountcreationfailed").ToString() + " !!! " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Edit(int id,string name, string userName, string des, bool status, bool authorities)
        {
            try
            {
                var user = db.Users.Find(id);
                user.Name = name;
                user.User1 = userName;
                user.Description = des;
                user.Status = status;
                user.Admin = authorities;
                db.SaveChanges();
                return Json(new { code = 200, msg = rm.GetString("editaccountsuccessfully").ToString() + " !!!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("fixfaultyaccount").ToString() + " !!! " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ChangePass(string oldPass,string newPass )
        {
            try
            {
                var user = (User)Session["user"];
                if(user.Pass != Encode.ToMD5(oldPass))
                {
                    return Json(new { code = 300, msg = rm.GetString("incorrectpassword").ToString() + " !!!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var user1 = db.Users.Find(user.Id);
                    user1.Pass = Encode.ToMD5(newPass);
                    db.SaveChanges();
                    return Json(new { code = 200, msg  = rm.GetString("changepasswordsuccessfully").ToString() +  " !!!" }, JsonRequestBehavior.AllowGet);
                }       
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("passwordfixfailed").ToString() + " !!! " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {

                Dele.DeleteUsers(id);
                    return Json(new { code = 200, msg = rm.GetString("accountdeletedsuccessfully").ToString()+" !!!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { code = 500,msg = rm.GetString("failedaccountdeletion").ToString() + " !!! " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}