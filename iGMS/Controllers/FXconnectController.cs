using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Resources;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Zebra_RFID_Scanner.Models;
using OfficeOpenXml;

namespace Zebra_RFID_Scanner.Controllers
{
    public class FXconnectController : BaseController
    {
        private Entities db = new Entities();
        // GET: FXconnect
        ResourceManager rm = new ResourceManager("Zebra_RFID_Scanner.Resources.Resource", typeof(Resources.Resource).Assembly);
        // GET: Size
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Adds()
        {
            return View();
        }
        public ActionResult Edits(string id)
        {
            if (id.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FXconnect fXconnect = db.FXconnects.Find(id);
            if (fXconnect == null)
            {
                return HttpNotFound();
            }
            return View(fXconnect);
        }
        [HttpGet]
        public JsonResult List(int pagenum, int page, string seach)
        {
            try
            {
                var pageSize = pagenum;
                var a = (from b in db.FXconnects.Where(x => x.Id.Length > 0 && x.Name != null)
                         select new
                         {
                             id = b.Id,
                             name = b.Name,
                         }).ToList().Where(x => x.name.ToLower().Contains(seach) || x.name.Contains(seach)
                                                || x.id.ToLower().Contains(seach) || x.id.Contains(seach));
                var pages = a.Count() % pageSize == 0 ? a.Count() / pageSize : a.Count() / pageSize + 1;
                var c = a.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var count = a.Count();
                return Json(new { code = 200, c = c, pages = pages, count = count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false").ToString() + " !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult FX()
        {
            try
            {
                var a = (from b in db.FXconnects.Where(x => x.Id.Length >0)
                         select new
                         {
                             id = b.Id,
                             name = b.Name,
                         }).ToList();
                return Json(new { code = 200, a=a }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false").ToString() + " !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Add(string name, string id)
        {
            try
            {
                var session = (User)Session["user"];
                var nameAdmin = session.Name;
                var d = new FXconnect();
                var checkFX = db.FXconnects.Find(id);
                if (checkFX == null)
                {
                    d.Id = id;
                    d.Name = name;
                    db.FXconnects.Add(d);
                    db.SaveChanges();
                    return Json(new { code = 200, msg = rm.GetString("successfullyaddfx").ToString() + " !!!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 300, msg = rm.GetString("coincide").ToString() + ", " + rm.GetString("alreadyhavecode").ToString() + " " + id + " " + rm.GetString("inthesystem").ToString() + " !!!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("addfxfailed").ToString() + " !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Edit(string id, string name)
        {
            try
            {
                var session = (User)Session["user"];
                var nameAdmin = session.Name;
                var d = db.FXconnects.Find(id);
                d.Name = name;
                db.SaveChanges();
                return Json(new { code = 200, msg = rm.GetString("fixfxsuccessfully").ToString() + " !!!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("fxfixfailed").ToString() + " !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Delete(string id)
        {
            try
            {
                Dele.DeleteFXconnects(id);
                return Json(new { code = 200, msg = rm.GetString("deletefxsuccessfully").ToString() + " !!!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { code = 500, msg = rm.GetString("deletefxfailed").ToString() + " !!!" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string currentDirectory = HostingEnvironment.MapPath("~");
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        ExcelWorksheet currentSheet = package.Workbook.Worksheets.First();
                        var workSheet = currentSheet;
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        ImportError[] importError = new ImportError[noOfRow];
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            try
                            {
                                if (workSheet.Cells[rowIterator, 1].Value != null)
                                {
                                    var id = workSheet.Cells[rowIterator, 1].Value == null ? null : workSheet.Cells[rowIterator, 1].Value.ToString();
                                    var name = workSheet.Cells[rowIterator, 2].Value == null ? null : workSheet.Cells[rowIterator, 2].Value.ToString();
                                    if (name == null)
                                    {
                                        importError[rowIterator - 1] = new ImportError(rm.GetString("nonamesentered").ToString(), rowIterator);
                                        continue;
                                    }
                                    if (id == null)
                                    {
                                        importError[rowIterator - 1] = new ImportError(rm.GetString("havenotenteredthecode").ToString(), rowIterator);
                                        continue;
                                    }
                                    var checkFXconnects = db.FXconnects.SingleOrDefault(x => x.Id == id);
                                    if (checkFXconnects == null)
                                    {
                                        var user = (User)Session["user"];
                                        var fXconnect = new FXconnect();
                                        fXconnect.Id = id;
                                        fXconnect.Name = name;
                                        db.FXconnects.Add(fXconnect);
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        importError[rowIterator - 1] = new ImportError(rm.GetString("alreadyhavecode").ToString(), rowIterator);
                                        continue;
                                    }
                                }
                            }
                            catch (DbEntityValidationException ex)
                            {
                                foreach (var error in ex.EntityValidationErrors)
                                {
                                    foreach (var validationError in error.ValidationErrors)
                                    {
                                        Console.WriteLine("Lỗi xác thực: {0}", validationError.ErrorMessage);
                                    }
                                }
                            }
                        }
                        importError = importError.Where(enem => enem != null).ToArray();
                        return Json(new { code = 200, msg = importError.Select(i => i.ToString()).ToList() }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { code = 300, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false").ToString() + " !!!" + e.Message }, JsonRequestBehavior.AllowGet);

            }

        }
    }
}