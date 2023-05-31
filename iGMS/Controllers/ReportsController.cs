using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Windows.Media;
using Zebra_RFID_Scanner.Models;

namespace Zebra_RFID_Scanner.Controllers
{
    public class ReportsController : BaseController
    {
        private Entities db = new Entities();
        ResourceManager rm = new ResourceManager("Zebra_RFID_Scanner.Resources.Resource", typeof(Resources.Resource).Assembly);
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult UnconfirmedReports(string name, string date)
        {
            try
            {
                string date1 = date==""?"": date.Substring(0, date.IndexOf("/")).Trim();
                string date2 = date == "" ? "" : date.Substring(date.IndexOf("/")+1).Trim();
                var s1= sumDate(date1);
                var s2 = sumDate(date2);
                var reports = (from b in db.Reports.Where(x => x.Status == false)
                               select new
                               {
                                   id = b.Id,
                                   createDate = b.CreateDate,
                                   createBy = b.CreateBy
                               }).ToList();
                if (name != "" && date != "")
                {
                    if (name == "-1")
                    {
                        var reportss = reports.Where(x => (Convert.ToInt32(x.createDate.Value.Day) + (Convert.ToInt32(x.createDate.Value.Month) * 30) + Convert.ToInt32(x.createDate.Value.Year)) >= s1&&
                                                         (Convert.ToInt32(x.createDate.Value.Day) + (Convert.ToInt32(x.createDate.Value.Month) * 30) + Convert.ToInt32(x.createDate.Value.Year)) <= s2);
                        return Json(new { code = 200, reports = reportss }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var user = db.Users.SingleOrDefault(x => x.Id.ToString() == name);
                        name = user.Name;
                        var reportss = reports.Where(x => (Convert.ToInt32(x.createDate.Value.Day) + (Convert.ToInt32(x.createDate.Value.Month) * 30) + Convert.ToInt32(x.createDate.Value.Year)) >= s1 && x.createBy == name
                        && (Convert.ToInt32(x.createDate.Value.Day) + (Convert.ToInt32(x.createDate.Value.Month) * 30) + Convert.ToInt32(x.createDate.Value.Year)) <= s2);
                        return Json(new { code = 200, reports = reportss }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { code = 200, reports = reports }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + " " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult confirmedReports(string name, string date)
        {
            try
            {
                string date1 = date == "" ? "" : date.Substring(0, date.IndexOf("/")).Trim();
                string date2 = date == "" ? "" : date.Substring(date.IndexOf("/") + 1).Trim();
                var s1 = sumDate(date1);
                var s2 = sumDate(date2);
                var reports = (from b in db.Reports.Where(x => x.Status == true)
                               select new
                               {
                                   id = b.Id,
                                   createDate = b.ModifyDate,
                                   createBy = b.ModifyBy
                               }).ToList();
                if (name != "" && date != "")
                {
                    if (name == "-1")
                    {
                        var reportss = reports.Where(x => (Convert.ToInt32(x.createDate.Value.Day) + (Convert.ToInt32(x.createDate.Value.Month) * 30) + Convert.ToInt32(x.createDate.Value.Year)) >= s1
                        && (Convert.ToInt32(x.createDate.Value.Day) + (Convert.ToInt32(x.createDate.Value.Month) * 30) + Convert.ToInt32(x.createDate.Value.Year)) <= s2);
                        return Json(new { code = 200, reports = reportss }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var user = db.Users.SingleOrDefault(x => x.Id.ToString() == name);
                        name = user.Name;
                        var reportss = reports.Where(x => (Convert.ToInt32(x.createDate.Value.Day) + (Convert.ToInt32(x.createDate.Value.Month) * 30) + Convert.ToInt32(x.createDate.Value.Year)) >= s1 && x.createBy == name
                        && (Convert.ToInt32(x.createDate.Value.Day) + (Convert.ToInt32(x.createDate.Value.Month) * 30) + Convert.ToInt32(x.createDate.Value.Year)) <= s2);
                        return Json(new { code = 200, reports = reportss }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { code = 200, reports = reports }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + " " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult UserReports()
        {
            try
            {
                var reports = (from b in db.Users.Where(x => x.Id > 0)
                               select new
                               {
                                   name = b.Name,
                                   id = b.Id
                               }).ToList();
                return Json(new { code = 200, reports = reports }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + " " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Remove(string id)
        {
            try
            {
                Dele.DeleteReports(id);
                return Json(new { code = 200, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + " " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult InfoReports(string id)
        {
            try
            {
                var createDates = db.Reports.SingleOrDefault(x => x.Id == id).CreateDate;
                var modifyDates = db.Reports.SingleOrDefault(x => x.Id == id).ModifyDate;
                var createDate = createDates.Value.Day + "-" + createDates.Value.Month + "-" + createDates.Value.Year + " " + createDates.Value.Hour + ":" + createDates.Value.Minute;
                var modifyDate = modifyDates == null ? "" : modifyDates.Value.Day + "-" + modifyDates.Value.Month + "-" + modifyDates.Value.Year + " " + modifyDates.Value.Hour + ":" + modifyDates.Value.Minute;
                var createBy = db.Reports.SingleOrDefault(x => x.Id == id).CreateBy;
                var Consignee = db.Reports.SingleOrDefault(x => x.Id == id).Name;
                var po = db.Reports.SingleOrDefault(x => x.Id == id).Po;
                var so = db.Reports.SingleOrDefault(x => x.Id == id).So;
                var sku = db.Reports.SingleOrDefault(x => x.Id == id).Sku;
                var Ctn = (from a in db.DataScanPhysicals.Where(x => x.IdReports == id)
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
                var epcToUpc = (from a in db.Data.Where(x => x.IdReports == id)
                                select new
                                {
                                    so=a.So,
                                    po=a.Po,
                                    sku=a.Sku,
                                    epc = a.EPC,
                                    upc = a.UPC
                                }).ToList();
                var discrepancies = (from a in db.Discrepancies.Where(x => x.IdReports == id)
                                     select new
                                     {
                                         so = a.So,
                                         po = a.Po,
                                         sku = a.Sku,
                                         ctn = a.CartonTo,
                                         upc = a.UPC,
                                         qty = a.Qty,
                                         qtyScan = a.QtyScan
                                     }).ToList();
                var general = (from a in db.Generals.Where(x => x.IdReports == id)
                               select new
                               {
                                   so = a.So,
                                   po = a.Po,
                                   sku = a.Sku,
                                   ctn = a.CartonTo,
                                   upc = a.UPC,
                                   qty = a.Qty,
                               }).ToList();
                return Json(new { code = 200, createDate = createDate, createBy = createBy, Ctn = Ctn, epcToUpc = epcToUpc, po = po, so = so, sku = sku,
                    discrepancies = discrepancies, general = general, modifyDate = modifyDate,
                    Consignee= Consignee
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false") + " " + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult confirmHod(string id)
        {
            try
            {
                var user = (User)Session["user"];
                var name = user.Name;
                var date = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
                var reports = db.Reports.SingleOrDefault(x => x.Id == id);
                reports.Status = true;
                reports.ModifyDate = DateTime.Now;
                reports.ModifyBy = name;
                db.SaveChanges();
                return Json(new { code = 200, date = date, id = id }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult reportGeneral(string[] data,string name,string hod)
        {
            try
            {
                hod = hod.Substring(0, hod.IndexOf(" "));
                var datas = JsonConvert.DeserializeObject<string[][]>(data[0]);
                string folderName = @"C:\RFID\Reports\";
                string filePath =  "General_"+ name +"_"+hod+".xlsx";
                string folderPath = folderName + filePath;
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    // Add a new worksheet to the workbook
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

                    // Add some content to the worksheet
                    for (int i = 0; i < datas.Length; i++)
                    {
                        for (int j = 0; j < datas[0].Length; j++)
                        {
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Value = datas[i][j];
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Left.Style = ExcelBorderStyle.Hair;
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                            if (i == 0) // Nếu là hàng đầu tiên
                            {
                                worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 0));
                            }
                        }
                    }
                    worksheet.View.FreezePanes(2, 1);
                    // Save the ExcelPackage to a file
                    FileInfo file = new FileInfo(folderPath);
                    excelPackage.SaveAs(file);
                }
                return Json(new { code = 200, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = " !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult reportDiscrepancy(string[] data,string name, string hod)
        {
            try
            {
                hod = hod.Substring(0, hod.IndexOf(" "));
                var datas = JsonConvert.DeserializeObject<string[][]>(data[0]);
                string folderName = @"C:\RFID\Reports\";
                string filePath = "Discrepancy_"+name+"_"+hod+".xlsx";
                string folderPath = folderName + filePath;
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    // Add a new worksheet to the workbook
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

                    // Add some content to the worksheet
                    for (int i = 0; i < datas.Length; i++)
                    {
                        for (int j = 0; j < datas[0].Length; j++)
                        {
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Value = datas[i][j];
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Left.Style = ExcelBorderStyle.Hair;
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                            if (i == 0) // Nếu là hàng đầu tiên
                            {
                                worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 0));
                            }
                        }
                    }
                    worksheet.View.FreezePanes(2, 1);
                    // Save the ExcelPackage to a file
                    FileInfo file = new FileInfo(folderPath);
                    excelPackage.SaveAs(file);
                }
                return Json(new { code = 200, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = " !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult reportEPC(string[] data,string name, string hod)
        {
            try
            {
                hod = hod.Substring(0, hod.IndexOf(" "));
                var datas = JsonConvert.DeserializeObject<string[][]>(data[0]);
                string folderName = @"C:\RFID\Reports\";
                string filePath = "EPC_"+name+"_"+hod+".xlsx";
                string folderPath = folderName + filePath;
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    // Add a new worksheet to the workbook
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

                    // Add some content to the worksheet
                    for (int i = 0; i < datas.Length; i++)
                    {
                        for (int j = 0; j < datas[0].Length; j++)
                        {
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Value = datas[i][j];
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Left.Style = ExcelBorderStyle.Hair;
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                            if (i == 0) // Nếu là hàng đầu tiên
                            {
                                worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 0));
                            }
                        }
                    }
                    worksheet.View.FreezePanes(2, 1);
                    // Save the ExcelPackage to a file
                    FileInfo file = new FileInfo(folderPath);
                    excelPackage.SaveAs(file);
                }
                return Json(new { code = 200, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult reportAll(string[] dataGeneral, string[] dataDiscrepancy, string[] dataEpc,string name, string hod)
        {
            try
            {
                hod = hod.Substring(0, hod.IndexOf(" "));
                var dataGenerals = JsonConvert.DeserializeObject<string[][]>(dataGeneral[0]);
                var dataDiscrepancys = JsonConvert.DeserializeObject<string[][]>(dataDiscrepancy[0]);
                var dataEpcs = JsonConvert.DeserializeObject<string[][]>(dataEpc[0]);
                string folderName = @"C:\RFID\Reports\";
                string filePath = name+"_"+hod+".xlsx";
                string folderPath = folderName + filePath;
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    // Add a new worksheet to the workbook
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("General");

                    // Add some content to the worksheet
                    for (int i = 0; i < dataGenerals.Length; i++)
                    {
                        for (int j = 0; j < dataGenerals[0].Length; j++)
                        {
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Value = dataGenerals[i][j];
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Left.Style = ExcelBorderStyle.Hair;
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                            worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                            if (i == 0) // Nếu là hàng đầu tiên
                            {
                                worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 0));
                            }
                        }
                    }
                    worksheet.View.FreezePanes(2, 1);
                    ExcelWorksheet worksheet2 = excelPackage.Workbook.Worksheets.Add("Discrepancy");

                    // Add some content to the worksheet
                    for (int i = 0; i < dataDiscrepancys.Length; i++)
                    {
                        for (int j = 0; j < dataDiscrepancys[0].Length; j++)
                        {
                            worksheet2.Cells[GetExcelColumnName(j + 1) + (i + 1)].Value = dataDiscrepancys[i][j];
                            worksheet2.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                            worksheet2.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            worksheet2.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                            worksheet2.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            worksheet2.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Left.Style = ExcelBorderStyle.Hair;
                            worksheet2.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            worksheet2.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                            worksheet2.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                            if (i == 0) // Nếu là hàng đầu tiên
                            {
                                worksheet2.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet2.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 0));
                            }
                        }
                    }
                    worksheet2.View.FreezePanes(2, 1);
                    ExcelWorksheet worksheet3 = excelPackage.Workbook.Worksheets.Add("Epc-Upc");

                    // Add some content to the worksheet
                    for (int i = 0; i < dataEpcs.Length; i++)
                    {
                        for (int j = 0; j < dataEpcs[0].Length; j++)
                        {
                            worksheet3.Cells[GetExcelColumnName(j + 1) + (i + 1)].Value = dataEpcs[i][j];
                            worksheet3.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                            worksheet3.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            worksheet3.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                            worksheet3.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            worksheet3.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Left.Style = ExcelBorderStyle.Hair;
                            worksheet3.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            worksheet3.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                            worksheet3.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                            if (i == 0) // Nếu là hàng đầu tiên
                            {
                                worksheet3.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet3.Cells[GetExcelColumnName(j + 1) + (i + 1)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 0));
                            }
                        }
                    }
                    worksheet3.View.FreezePanes(2, 1);
                    // Save the ExcelPackage to a file
                    FileInfo file = new FileInfo(folderPath);
                    excelPackage.SaveAs(file);
                }
                return Json(new { code = 200, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpPost]
        public JsonResult Save(string arr,string hodReports)
        {
            try
            {
                var user = (User)Session["user"];
                var name = user.Name;
                var datas = JsonConvert.DeserializeObject<EPCTOUPC.EpctoUpc[]>(arr);
                for (int i = 0; i < datas.Length; i++)
                {
                    var u = datas[i].UPC;
                    var D = db.Discrepancies.Where(x=>x.UPC == u && x.IdReports==hodReports).ToList().LastOrDefault();
                    var G = db.Generals.Where(x => x.UPC == u && x.IdReports == hodReports).ToList().LastOrDefault();
                    string So, Po, Sku;
                    if (D == null)
                    {
                        So = G.So;
                        Po = G.Po;
                        Sku = G.Sku;
                    }
                    else
                    {
                        So = D.So;
                        Po = D.Po;
                        Sku = D.Sku;
                    }
                    var checkEpc = db.Data.Find(datas[i].EPC);
                    if (checkEpc == null)
                    {
                        Datum datum = new Datum();
                        datum.So = So;
                        datum.Po = Po;
                        datum.Sku = Sku;
                        datum.EPC = datas[i].EPC;
                        datum.IdReports = hodReports;
                        datum.UPC = datas[i].UPC;
                        datum.CreateDate = DateTime.Now;
                        datum.CreateBy = name;
                        db.Data.Add(datum);
                        db.SaveChanges();
                    }
                    else
                    {

                    }
                }
                HashSet<string> upcSet = new HashSet<string>();
                foreach (var obj in datas)
                {
                    string upc = (string)obj.UPC;
                    upcSet.Add(upc);
                }
                string[] uniqueUPCs = upcSet.ToArray();

                foreach(var item in uniqueUPCs)
                {
                    var count = db.Discrepancies.Where(x => x.UPC == item&&x.IdReports==hodReports).Count();
                    if (count == 0)
                    {
                        continue;
                    }
                    for(int i = 0; i < count; i++)
                    {
                        var dis = db.Discrepancies.OrderBy(x => x.UPC == item && x.IdReports == hodReports).ToList().LastOrDefault();
                        var ctn = dis.CartonTo;
                        var qty = dis.Qty;
                        General general = new General()
                        {
                            IdReports = hodReports,
                            CartonTo = ctn,
                            UPC = item,
                            Qty = qty,
                            CreateDate = DateTime.Now,
                            CreateBy = name
                        };
                        db.Generals.Add(general);
                        db.Discrepancies.Remove(dis);
                        db.SaveChanges();
                    }
                }
                return Json(new { code = 200, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = "Sai !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Upload(HttpPostedFileBase file,string[] dataDiscrepancy)
        {
            try
            {
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    var dataDiscrepancys = JsonConvert.DeserializeObject<string[][]>(dataDiscrepancy[0]);
                    string currentDirectory = HostingEnvironment.MapPath("~");
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    string[] upcDiscrepancy = new string[0];
                    for (int i = 1; i < dataDiscrepancys.Length; i++)
                    {
                        int index = Array.IndexOf(upcDiscrepancy, Array.Find(upcDiscrepancy, element => element.Contains(dataDiscrepancys[i][4])));
                        if (index >= 0)
                        {
                            var sl = upcDiscrepancy[index].Substring(upcDiscrepancy[index].IndexOf("-") + 1);
                            upcDiscrepancy[index] = dataDiscrepancys[i][4] + "-" + (int.Parse(dataDiscrepancys[i][5]) + int.Parse(sl));
                        }
                        else
                        {
                            Array.Resize(ref upcDiscrepancy, upcDiscrepancy.Length + 1);
                            upcDiscrepancy[upcDiscrepancy.Length - 1] = dataDiscrepancys[i][4] + "-" + dataDiscrepancys[i][5];
                        }
                    }
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        ExcelWorksheet currentSheet = package.Workbook.Worksheets.First();
                        var workSheet = currentSheet;
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        List<Gerenal.gerenal> generals = new List<Gerenal.gerenal>();
                        List<EPCTOUPC.EpctoUpc> epctoUpcs = new List<EPCTOUPC.EpctoUpc>();
                        for (int rowIterator = 7; rowIterator <= noOfRow; rowIterator++)
                        {
                            try
                            {
                                var epc = workSheet.Cells[rowIterator, 1].Value == null ? null : workSheet.Cells[rowIterator, 1].Value.ToString();
                                if (epc == null)
                                {
                                    continue;
                                }
                                EPCTOUPC.EpctoUpc epctoUpc = new EPCTOUPC.EpctoUpc()
                                {
                                    EPC = epc,
                                    UPC = epctoupc(epc)
                                };
                                epctoUpcs.Add(epctoUpc);
                                if (generals.Any(item => item.Upc == epctoupc(epc)))
                                {
                                    var general = generals.SingleOrDefault(x => x.Upc == epctoupc(epc));
                                    int qty = int.Parse(general.Qty);
                                    qty++;
                                    general.Qty = qty.ToString();
                                }
                                else
                                {
                                    Gerenal.gerenal gerenal = new Gerenal.gerenal()
                                    {
                                        Upc = epctoupc(epc),
                                        Qty = "1",
                                        Status = true
                                    };
                                    generals.Add(gerenal);
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
                        foreach(var item in upcDiscrepancy)
                        {
                            var upc = item.Substring(0, item.IndexOf("-"));
                            var qty = item.Substring(item.IndexOf("-")+1);
                            if (generals.Any(x => x.Upc == upc))
                            {
                                if (int.Parse(generals.SingleOrDefault(x => x.Upc == upc).Qty) == int.Parse(qty))
                                {
                                    foreach (var status in epctoUpcs)
                                    {
                                        if (status.UPC == upc)
                                        {
                                            status.Status = "Matched";
                                            status.Qty = generals.SingleOrDefault(x => x.Upc == upc).Qty+"/"+qty;
                                        }
                                    }
                                }else if(int.Parse(generals.SingleOrDefault(x => x.Upc == upc).Qty) > int.Parse(qty))
                                {
                                    foreach (var status in epctoUpcs)
                                    {
                                        if (status.UPC == upc)
                                        {
                                            status.Status = "Mismatched";
                                            status.Qty = generals.SingleOrDefault(x => x.Upc == upc).Qty + "/" + qty;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var status in epctoUpcs)
                                    {
                                        if (status.UPC == upc)
                                        {
                                            status.Status = "Mismatched";
                                            status.Qty = generals.SingleOrDefault(x => x.Upc == upc).Qty + "/" + qty;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (var status in epctoUpcs)
                                {
                                    if (status.UPC == upc)
                                    {
                                        status.Status = "error";
                                        status.Qty = generals.SingleOrDefault(x => x.Upc == upc).Qty + "/" + qty;
                                    }
                                }
                            }
                        }
                        return Json(new { code = 200,msg=epctoUpcs.ToList()  }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { code = 300, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { code = 500, msg = rm.GetString("false").ToString() + " !!!" + e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = string.Empty;
            int modulo;
            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        public static int sumDate(string date)
        {
            var y = date == "" ? "" : date.Substring(0, date.IndexOf("-"));
            var m = date == "" ? "" : date.Substring(date.IndexOf("-") + 1, 2);
            var d = date == "" ? "" : date.Substring(date.LastIndexOf("-") + 1,2);
            var s = date == "" ? 0 : int.Parse(d) + (int.Parse(m) * 30) + int.Parse(y);
            return s;
        }
        public static string epctoupc(string epc)
        {
        
            string EPC = epc;
            string EPCIN = "";
            string SGTIN = "";
            string ItemRef = "";
            string Result = "";
            string UPC = "";
            int i, SGTINResult, ItemRefResult, CheckDigit = 0;

            for (i = 0; i < EPC.Length; i++)
            {
                EPCIN += Convert.ToString(Convert.ToInt32(EPC.Substring(i, 1), 16), 2).PadLeft(4, '0');
            }

            EPCIN = EPCIN.Substring(EPCIN.Length - 82);
            SGTIN = EPCIN.Substring(0, 24);
            ItemRef = EPCIN.Substring(24, 20);

            SGTINResult = 0;
            for (i = 1; i < SGTIN.Length; i++)
            {
                SGTINResult += Convert.ToInt32(SGTIN.Substring(i, 1)) * (int)Math.Pow(2, SGTIN.Length - i - 1);
            }

            ItemRefResult = 0;
            for (i = 1; i < ItemRef.Length; i++)
            {
                ItemRefResult += Convert.ToInt32(ItemRef.Substring(i, 1)) * (int)Math.Pow(2, ItemRef.Length - i - 1);
            }

            Result = SGTINResult.ToString() + ("00000" + ItemRefResult).Substring(Math.Max(0, ("00000" + ItemRefResult).Length - 5));

            CheckDigit = 0;
            for (i = 1; i <= 17; i++)
            {
                if (Result.Length > Math.Abs(i - 17))
                {
                    if (i % 2 != 0)
                    {
                        CheckDigit += 3 * Convert.ToInt32(Result.Substring(Result.Length - Math.Abs(i - 17) - 1, 1));
                    }
                    else
                    {
                        CheckDigit += Convert.ToInt32(Result.Substring(Result.Length - Math.Abs(i - 17) - 1, 1));
                    }
                }
            }

            CheckDigit = Convert.ToInt32(Math.Ceiling((double)CheckDigit / 10) * 10) - CheckDigit;
            UPC = Result + CheckDigit;

            return UPC;
        }

    }
}