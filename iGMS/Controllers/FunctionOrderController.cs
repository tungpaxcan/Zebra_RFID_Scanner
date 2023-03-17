using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows;
using CollTex.Models;
using OfficeOpenXml;

namespace CollTex.Controllers
{
    public class FunctionOrderController : BaseController
    {
        private ColltexEntities db = new ColltexEntities();
        ResourceManager rm = new ResourceManager("CollTex.Resources.Resource", typeof(Resources.Resource).Assembly);
        // GET: FunctionOrder
        public ActionResult CardRecognition()
        {
            return View();
        }
        public ActionResult Index()
        {
       
            return View();
        }
        [HttpPost]
        public string UploadImage(HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string _FileName = "";
                    _FileName = /*sub11 + sub21 + */file.FileName;
                    file.SaveAs(Server.MapPath("/Images/" + _FileName));
                    return _FileName;
                }

            }
            return "";

        }
        [HttpGet]
        public JsonResult list(string seach)
        {
            try
            {
                var b = rm.GetString("unchecked").ToString();
                var c = rm.GetString("checked").ToString();

                var pkl = (from a in db.PKLs.Where(x => x.Id > 0)
                           select new
                           {
                               id = a.Id,
                               po = a.Name,
                               link = a.Link,
                               status = a.Status == true ? b+ "-danger" : c + "-success",
                               date = a.CreateDate.Value.Day + "/" + a.CreateDate.Value.Month + "/" + a.CreateDate.Value.Year
                           }).ToList().Where(x=>x.date.Contains(seach)||x.po.Contains(seach)||x.po.ToLower().Contains(seach));
                return Json(new { code = 200,pkl = pkl }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Add(string name)
        {
            try
            {
                var pkl = new PKL();
                pkl.Name = name;
                pkl.Link = name;
                pkl.Status = true;
                pkl.CreateDate = DateTime.Now;
                pkl.ModifyDate = DateTime.Now;
                db.PKLs.Add(pkl);
                db.SaveChanges();
                return Json(new { code = 200,  }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Status(int id)
        {
            try
            {
                var pkl = db.PKLs.Find(id);
                pkl.Status = false;
                db.SaveChanges();
                return Json(new { code = 200, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var pkl = db.PKLs.Find(id);
                db.PKLs.Remove(pkl);
                db.SaveChanges();
                System.IO.File.Delete(Server.MapPath("/Images/"+pkl.Link));
                return Json(new { code = 200, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
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
                                var id = workSheet.Cells[rowIterator, 1].Value == null ? null : workSheet.Cells[rowIterator, 1].Value.ToString();
                                var epc = workSheet.Cells[rowIterator, 2].Value == null ? null : workSheet.Cells[rowIterator, 2].Value.ToString();
                                if (id == null)
                                {
                                    importError[rowIterator-1] = new ImportError( rm.GetString("noproductcodeentered").ToString(), rowIterator);
                                    continue;
                                }
                                if (epc == null)
                                {
                                    importError[rowIterator-1] = new ImportError(rm.GetString("donimportepc").ToString(), rowIterator);
                                    continue;
                                }
                                var checkEpc = db.EPCs.SingleOrDefault(x => x.IdEPC == epc);
                                if (checkEpc == null)
                                {
                                    var user = (User)Session["user"];
                                    var ePC = new EPC();
                                    ePC.IdGoods = id;
                                    ePC.IdEPC = epc;
                                    ePC.Status = true;
                                    db.EPCs.Add(ePC);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    importError[rowIterator-1] = new ImportError(rm.GetString("alreadyhaveEPC").ToString(), rowIterator);
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
                return Json(new { code = 300,}, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false").ToString()+" !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}