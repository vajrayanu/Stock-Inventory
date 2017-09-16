using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ITST.Models;
using ITST.ViewModels;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Data.Entity.Core.Objects;

namespace ITST.Controllers
{
    public class UserSheetController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /UserSheet/
        [Authorize]
        public ActionResult Index()
        {
            //return View(db.UserSheets.ToList());
            return View();
        }

        public JsonResult getUserSheet()
        {
            var sheet = db.UserSheets.OrderByDescending(p => p.DateUpdate).ToList();
            return Json(new { data = sheet }, JsonRequestBehavior.AllowGet);
        }

        // GET: /UserSheet/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSheet usersheet = db.UserSheets.Find(id);
            if (usersheet == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = id;
            return View(usersheet);
        }

        public ActionResult ExportUserSheetPdf(int? id)
        {
            return new Rotativa.ActionAsPdf("ExportPdf", new { id })
            {
                PageSize = Rotativa.Options.Size.A3,
                PageMargins = new Rotativa.Options.Margins(0, 0, 0, 0)
            };
        }

        public ActionResult ExportFormSheetPdf()
        {
            return new Rotativa.ActionAsPdf("ExportFormPdf")
            {
                PageSize = Rotativa.Options.Size.A3,
                PageMargins = new Rotativa.Options.Margins(0,0,0,0)
            };
        }

        public ActionResult ExportFormPdf()
        {
            return View();
        }

        public ActionResult ExportPdf(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSheet usersheet = db.UserSheets.Find(id);
            if (usersheet == null)
            {
                return HttpNotFound();
            }
            return View(usersheet);
        }

        public ActionResult summaryRpt()
        {
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentName", "DepartmentName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult summaryRpt([Bind(Include = "StartDate,EndDate,DepartmentID")] SummaryRpt summaryrpt)
        {
            if (ModelState.IsValid)
            {
                if (summaryrpt.StartDate == null)
                {
                    return View(summaryrpt);
                }
                else if (summaryrpt.EndDate == null)
                {
                    return RedirectToAction("summaryRptStartDate", "MachineSheet", summaryrpt);
                }
                else
                {
                    return RedirectToAction("summaryRptExport", "UserSheet", summaryrpt);
                }
            }
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentName", "DepartmentName");
            return View(summaryrpt);
        }

        public ActionResult summaryRptExport([Bind(Include = "StartDate,EndDate,DepartmentID")] SummaryRpt summaryrpt)
        {
            if (ModelState.IsValid)
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Reports"), "SummaryUserSheet.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }
                List<UserSheet> cm = new List<UserSheet>();

                if (summaryrpt.EndDate > summaryrpt.StartDate)
                {
                    {
                        var query = from s in db.UserSheets
                                    orderby s.SheetID
                                    where (EntityFunctions.TruncateTime(s.DateCreate) >= EntityFunctions.TruncateTime(summaryrpt.StartDate) && s.Department == summaryrpt.DepartmentID && EntityFunctions.TruncateTime(s.DateCreate) <= EntityFunctions.TruncateTime(summaryrpt.EndDate) && s.Department == summaryrpt.DepartmentID)
                                    select s;
                        cm = query.ToList();
                    }
                }
                else
                {
                    var query = from s in db.UserSheets
                                orderby s.SheetID
                                where (EntityFunctions.TruncateTime(s.DateCreate) == EntityFunctions.TruncateTime(summaryrpt.StartDate) && s.Department == summaryrpt.DepartmentID && EntityFunctions.TruncateTime(s.DateCreate) <= EntityFunctions.TruncateTime(summaryrpt.EndDate) && s.Department == summaryrpt.DepartmentID)
                                select s;
                    cm = query.ToList();
                }

                ReportDataSource rd = new ReportDataSource("DataSet1", cm);
                lr.DataSources.Add(rd);
                string reportType = "Excel";
                string mimeType;
                string encoding;
                string fileNameExtension;

                string deviceInfo =

                "<DeviceInfo>" +
                "  <OutputFormat>" + "Excel" + "</OutputFormat>" +
                "  <PageWidth>11.7in</PageWidth>" +
                "  <PageHeight>16.5in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>1in</MarginLeft>" +
                "  <MarginRight>1in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                renderedBytes = lr.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);
                return File(renderedBytes, mimeType);
            }
            ViewBag.DepartmentID = new SelectList(db.Departments.OrderBy(d => d.DepartmentName), "DepartmentName", "DepartmentName");
            return View(summaryrpt);
        }


        // GET: /UserSheet/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /UserSheet/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="SheetID,PCBoardBrand,PCBoardModel,PCBoardSerial,PCBoardRemark,ProcessorBrand,ProcessorModel,ProcessorSerial,ProcessorRemark,MemoryBrand,MemoryModel,MemorySerial,MemoryRemark,VideoCardBrand,VideoCardModel,VideoCardSerial,VideoCardRemark,LANCardBrand,LANCardModel,LANCardSerial,LANCardRemark,MouseBrand,MouseModel,MouseSerial,MouseRemark,MonitorBrand,MonitorModel,MonitorSerial,MonitorRemark,Monitor2Brand,Monitor2Model,Monitor2Serial,Monitor2Remark,KeyboardBrand,KeyboardModel,KeyboardSerial,KeyboardRemark,ScannerBrand,ScannerModel,ScannerSerial,ScannerRemark,DVDBrand,DVDModel,DVDSerial,DVDRemark,HDD1Brand,HDD1Model,HDD1Serial,HDD1Remark,HDD2Brand,HDD2Model,HDD2Serial,HDD2Remark,PrinterBrand,PrinterModel,PrinterSerial,PrinterRemark,UPSBrand,UPSModel,UPSSerial,UPSRemark,Zip7Ver,Zip7Remark,AcrobatVer,AcrobatRemark,AutoCADVer,AutoCADRemark,CureGraphVer,CureGraphRemark,CutePDFVer,CutePDFRemark,HealthBookVer,HealthBookRemark,HRMSystemVer,HRMSystemRemark,IllustratorVer,IllustratorRemark,JP1Ver,JP1Remark,LexitronDictVer,LexitronDictRemark,MiniTabVer,MiniTabRemark,MSOfficeVer,MSOfficeRemark,PhotoShopVer,PhotoShopRemark,SAPVer,SAPRemark,SaveToPDFVer,SaveToPDFRemark,SeedWincsVer,SeedWincsRemark,SolidEdgeVer,SolidEdgeRemark,TrendMicroVer,TrendMicroRemark,WindowsVer,WindowsRemark,ZimbraVer,ZimbraRemark,OtherProgram,OtherProgramVer,OtherProgramRemark,OtherProgram1,OtherProgram1Ver,OtherProgram1Remark,OtherProgram2,OtherProgram2Ver,OtherProgram2Remark,UserID,UserName,PlantID,DepartmentID,LocationID,PhaseID,Plant,Department,Location,Phase,FixAccess,CreateBy,UpdateBy,DateCreate,DateUpdate,DeviceName")] UserSheet usersheet)
        {
            if (ModelState.IsValid)
            {
                db.UserSheets.Add(usersheet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usersheet);
        }

        // GET: /UserSheet/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSheet usersheet = db.UserSheets.Find(id);
            usersheet.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            usersheet.DateUpdate = DateTime.Now;
            if (usersheet == null)
            {
                return HttpNotFound();
            }
            return View(usersheet);
        }

        // POST: /UserSheet/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="SheetID,PCBoardBrand,PCBoardModel,PCBoardSerial,PCBoardRemark,ProcessorBrand,ProcessorModel,ProcessorSerial,ProcessorRemark,MemoryBrand,MemoryModel,MemorySerial,MemoryRemark,VideoCardBrand,VideoCardModel,VideoCardSerial,VideoCardRemark,LANCardBrand,LANCardModel,LANCardSerial,LANCardRemark,MouseBrand,MouseModel,MouseSerial,MouseRemark,MonitorBrand,MonitorModel,MonitorSerial,MonitorRemark,Monitor2Brand,Monitor2Model,Monitor2Serial,Monitor2Remark,KeyboardBrand,KeyboardModel,KeyboardSerial,KeyboardRemark,ScannerBrand,ScannerModel,ScannerSerial,ScannerRemark,DVDBrand,DVDModel,DVDSerial,DVDRemark,HDD1Brand,HDD1Model,HDD1Serial,HDD1Remark,HDD2Brand,HDD2Model,HDD2Serial,HDD2Remark,PrinterBrand,PrinterModel,PrinterSerial,PrinterRemark,UPSBrand,UPSModel,UPSSerial,UPSRemark,Zip7Ver,Zip7Remark,AcrobatVer,AcrobatRemark,AutoCADVer,AutoCADRemark,CureGraphVer,CureGraphRemark,CutePDFVer,CutePDFRemark,HealthBookVer,HealthBookRemark,HRMSystemVer,HRMSystemRemark,IllustratorVer,IllustratorRemark,JP1Ver,JP1Remark,LexitronDictVer,LexitronDictRemark,MiniTabVer,MiniTabRemark,MSOfficeVer,MSOfficeRemark,PhotoShopVer,PhotoShopRemark,SAPVer,SAPRemark,SaveToPDFVer,SaveToPDFRemark,SeedWincsVer,SeedWincsRemark,SolidEdgeVer,SolidEdgeRemark,TrendMicroVer,TrendMicroRemark,WindowsVer,WindowsRemark,ZimbraVer,ZimbraRemark,OtherProgram,OtherProgramVer,OtherProgramRemark,OtherProgram1,OtherProgram1Ver,OtherProgram1Remark,OtherProgram2,OtherProgram2Ver,OtherProgram2Remark,UserID,UserName,PlantID,DepartmentID,LocationID,PhaseID,Plant,Department,Location,Phase,FixAccess,CreateBy,UpdateBy,DateCreate,DateUpdate,DeviceName")] UserSheet usersheet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usersheet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usersheet);
        }

        // GET: /UserSheet/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSheet usersheet = db.UserSheets.Find(id);
            if (usersheet == null)
            {
                return HttpNotFound();
            }
            return View(usersheet);
        }

        // POST: /UserSheet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserSheet usersheet = db.UserSheets.Find(id);
            db.UserSheets.Remove(usersheet);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
