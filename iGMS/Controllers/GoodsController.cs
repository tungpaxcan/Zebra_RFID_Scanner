using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Windows;
using CollTex.Models;
using OfficeOpenXml;
using System.Text;
using System.Web.Hosting;
using System.Data.Entity.Validation;
using System.Resources;

namespace CollTex.Controllers
{
    public class GoodsController : BaseController
    {
        private ColltexEntities db = new ColltexEntities();
        ResourceManager rm = new ResourceManager("CollTex.Resources.Resource", typeof(Resources.Resource).Assembly);
        // GET: Goods
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
            if (id.Length <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Good good = db.Goods.Find(id);
            if (good == null)
            {
                return HttpNotFound();
            }
            return View(good);
        }
        public ActionResult Details(string id)
        {
            if (id.Length <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Good good = db.Goods.Find(id);
            if (good == null)
            {
                return HttpNotFound();
            }
            return View(good);
        }
        [HttpGet]
        public JsonResult CodeGoods()
        {
            try
            {
                var a = (from b in db.Goods.Where(x => x.Id.Length > 0)
                         select new
                         {
                             id = b.Id,
                             idgoods = b.IdGood,
                         }).ToList();
                return Json(new { code = 200, a=a }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult List(int pagenum, int page, string seach)
        {
            try
            {

                var pageSize = pagenum;
                var a = (from b in db.Goods.Where(x => x.Id.Length > 0)
                         select new
                         {
                             id = b.Id,
                             idgood = b.IdGood,
                             name = b.Name,
                         }).ToList().Where(x => x.id.ToLower().Contains(seach) || x.id.Contains(seach)
                                                ||x.name.ToLower().Contains(seach)||x.name.Contains(seach)
                                                ||x.idgood.ToLower().Contains(seach)||x.idgood.Contains(seach));
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
        [HttpPost]
        public JsonResult Add(string id, string idgood,string style, string color, string size, string name)
        {
            try
            {
                var session = (User)Session["user"];
                var nameAdmin = session.Name;
                var d = new Good();
                var checkGood = db.Goods.SingleOrDefault(x=>x.Id==id||x.IdGood==idgood);
                if (checkGood == null)
                {
                    d.Id = id;
                    d.IdGood = idgood;
                    d.IdStyle = style;
                    d.IdColor = color;
                    d.IdSize = size;
                    d.Name = name;
                    d.CreateBy = nameAdmin;
                    d.CreateDate = DateTime.Now;
                    d.ModifyBy = nameAdmin;
                    d.ModifyDate = DateTime.Now;
                    db.Goods.Add(d);
                    db.SaveChanges();
                    return Json(new { code = 200, msg =rm.GetString("createsuccessfulproducts").ToString()+ " !!!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 300, msg = rm.GetString("coincide").ToString()+ ", "+rm.GetString("alreadyhavecode").ToString() +" "+id+" "+ rm.GetString("product").ToString()+" "+ rm.GetString("inthesystem").ToString() + " !!!" }, JsonRequestBehavior.AllowGet);
                }
               
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("createdefectiveproducts").ToString() + " !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Edit(string id, string idgood, string style, string color, string size, string name)
        {
            try
            {
                var session = (User)Session["user"];
                var nameAdmin = session.Name;
                var d = db.Goods.Find(id);
                d.IdGood = idgood;
                d.IdStyle = style;
                d.IdColor = color;
                d.IdSize = size;
                d.Name = name;
                d.ModifyBy = nameAdmin;
                d.ModifyDate = DateTime.Now;
                db.SaveChanges();
                return Json(new { code = 200, msg = rm.GetString("successfulproductrepair").ToString() + " !!!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("fixproducterror").ToString() + " !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Delete(string id)
        {
            try
            {
                Dele.DeleteGoods(id);
                return Json(new { code = 200, msg = rm.GetString("deleteproductsuccessfully").ToString()+ " !!!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("deletedefectiveproduct").ToString() + " " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult Style()
        {
            try
            {
                var c = (from b in db.Styles.Where(x => x.Id.Length > 0)
                         select new
                         {
                             id = b.Id,
                             name = b.Name
                         }).ToList();
                return Json(new { code = 200, c = c, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false").ToString() + " !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult Color()
        {
            try
            {
                var c = (from b in db.Colors.Where(x => x.Id.Length > 0)
                         select new
                         {
                             id = b.Id,
                             name = b.Name
                         }).ToList();
                return Json(new { code = 200, c = c, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false").ToString() + " !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult Size()
        {
            try
            {
                var c = (from b in db.Sizes.Where(x => x.Id.Length > 0)
                         select new
                         {
                             id = b.Id,
                             name = b.Name
                         }).ToList();
                return Json(new { code = 200, c = c, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false").ToString() + " !!!" + e.Message }, JsonRequestBehavior.AllowGet);
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
                                    var style = workSheet.Cells[rowIterator, 2].Value == null ? null : workSheet.Cells[rowIterator, 2].Value.ToString();
                                    var color = workSheet.Cells[rowIterator, 3].Value == null ? null : workSheet.Cells[rowIterator, 3].Value.ToString();
                                    var size = workSheet.Cells[rowIterator, 4].Value == null ? null : workSheet.Cells[rowIterator, 4].Value.ToString();
                                    var idGood = workSheet.Cells[rowIterator, 5].Value == null ? null : workSheet.Cells[rowIterator, 5].Value.ToString();
                                    var name = workSheet.Cells[rowIterator, 6].Value == null ? null : workSheet.Cells[rowIterator, 6].Value.ToString();
                                    if (style == null)
                                    {
                                        importError[rowIterator - 1] = new ImportError(rm.GetString("stylehasnotbeenimported").ToString(), rowIterator);
                                        continue;
                                    }
                                    if (color == null)
                                    {
                                        importError[rowIterator - 1] = new ImportError(rm.GetString("colorsarenotimported").ToString(), rowIterator);
                                        continue;
                                    }
                                    if (size == null)
                                    {
                                        importError[rowIterator - 1] = new ImportError(rm.GetString("sizenotentered").ToString(), rowIterator);
                                        continue;
                                    }
                                    if (idGood == null)
                                    {
                                        importError[rowIterator - 1] = new ImportError(rm.GetString("noproductcodeentered").ToString(), rowIterator);
                                        continue;
                                    }
                                    if (name == null)
                                    {
                                        importError[rowIterator - 1] = new ImportError(rm.GetString("productnamehasnotbeenentered").ToString(), rowIterator);
                                        continue;
                                    }
                                    var checkStyle = db.Styles.Find(style);
                                    if (checkStyle == null)
                                    {
                                        CollTex.Models.Style sty = new CollTex.Models.Style();
                                        sty.Id = style;
                                        sty.Name = style;
                                        db.Styles.Add(sty);
                                        db.SaveChanges();
                                    }
                                    var checkColor = db.Colors.Find(color);
                                    if (checkColor == null)
                                    {
                                        Color c = new Color();
                                        c.Id = color;
                                        c.Name = color;
                                        db.Colors.Add(c);
                                        db.SaveChanges();
                                    }
                                    var checkSize = db.Sizes.Find(size);
                                    if (checkSize == null)
                                    {
                                        CollTex.Models.Size s = new CollTex.Models.Size();
                                        s.Id = size;
                                        s.Name = size;
                                        db.Sizes.Add(s);
                                        db.SaveChanges();
                                    }
                                    var checkGood = db.Goods.Find(id);
                                    if (checkGood == null)
                                    {
                                        var user = (User)Session["user"];
                                        var nameAdmin = user.Name;
                                        Good good = new Good();
                                        good.Id = id;
                                        good.IdGood = idGood;
                                        good.IdStyle = style;
                                        good.IdColor = color;
                                        good.IdSize = size;
                                        good.Name = name;
                                        good.CreateBy = nameAdmin;
                                        good.CreateDate = DateTime.Now;
                                        good.ModifyBy = nameAdmin;
                                        good.ModifyDate = DateTime.Now;
                                        db.Goods.Add(good);
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        importError[rowIterator - 1] = new ImportError(rm.GetString("alreadyhavecode").ToString(), rowIterator);
                                        continue;
                                    }
                                }
                                else
                                {
                                    importError[rowIterator - 1] = new ImportError(rm.GetString("theproductbarcodehasnotbeenentered").ToString(), rowIterator);
                                    continue;
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
            catch(Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false").ToString() + " !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            } 
        }
    }
}