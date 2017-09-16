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
    public class MachineSheetController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /MachineSheet/
        [Authorize]
        public ActionResult Index()
        {
            //return View(db.PMSheets.OrderByDescending(p=>p.DateUpdate).ToList());
            return View();
        }

        public JsonResult getPMSheet()
        {
            var pmsheet = db.PMSheets.OrderByDescending(p=>p.DateUpdate).ToList();
            return Json(new { data = pmsheet }, JsonRequestBehavior.AllowGet);
        }

        // GET: /MachineSheet/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PMSheet pmsheet = db.PMSheets.Find(id);
            if (pmsheet == null)
            {
                return HttpNotFound();
            }
            return View(pmsheet);
        }

        public ActionResult print(int? id)
        {
            if (ModelState.IsValid)
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Reports"), "RptPMSheet.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }

                List<PMSheet> cm = new List<PMSheet>();
                cm = db.PMSheets.Where(d => d.SheetID == id).ToList();

                ReportDataSource rd = new ReportDataSource("DataSet", cm);
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
            return View(id);
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
                    return RedirectToAction("summaryRptExport", "MachineSheet", summaryrpt);
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
                string path = Path.Combine(Server.MapPath("~/Reports"), "SummaryPMSheetRpt.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }
                List<PMSheet> cm = new List<PMSheet>();

                if (summaryrpt.EndDate > summaryrpt.StartDate)
                {
                    {
                        var query = from s in db.PMSheets
                                    orderby s.SheetID
                                    where (EntityFunctions.TruncateTime(s.DateCreate) >= EntityFunctions.TruncateTime(summaryrpt.StartDate) && s.DepartmentName == summaryrpt.DepartmentID && EntityFunctions.TruncateTime(s.DateCreate) <= EntityFunctions.TruncateTime(summaryrpt.EndDate) && s.DepartmentName == summaryrpt.DepartmentID)
                                    select s;
                        cm = query.ToList();
                    }
                }
                else
                {
                    var query = from s in db.PMSheets
                                orderby s.SheetID
                                where (EntityFunctions.TruncateTime(s.DateCreate) == EntityFunctions.TruncateTime(summaryrpt.StartDate) && s.DepartmentName == summaryrpt.DepartmentID && EntityFunctions.TruncateTime(s.DateCreate) <= EntityFunctions.TruncateTime(summaryrpt.EndDate) && s.DepartmentName == summaryrpt.DepartmentID)
                                select s;
                    cm = query.ToList();
                }

                ReportDataSource rd = new ReportDataSource("DataSet", cm);
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



        // GET: /MachineSheet/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PMSheet pmsheet = db.PMSheets.Find(id);
            var updateName = System.Web.HttpContext.Current.User.Identity.Name;
            pmsheet.UpdateBy = db.Users.Where(u => u.EmployeeID == updateName).Select(u => u.FullName).DefaultIfEmpty().First();
            pmsheet.DateUpdate = DateTime.Now;
            if (pmsheet == null)
            {
                return HttpNotFound();
            }
            return View(pmsheet);
        }

        // POST: /MachineSheet/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SheetID,PCBoardBrand,PCBoardModel,PCBoardSerial,PCBoardRemark,CreateBy,UpdateBy,DateCreate,DateUpdate,FixAccess,ProcessorBrand,ProcessorModel,ProcessorSerial,ProcessorRemark,PlantID,DepartmentID,LocationID,PhaseID,MachineID,PlantName,DepartmentName,LocationName,PhaseName,MachineName,MemoryBrand,MemoryModel,MemorySerial,MemoryRemark,VideoCardBrand,VideoCardModel,VideoCardSerial,VideoCardRemark,LANCardBrand,LANCardSerial,LANCardModel,LANCardRemark,MouseBrand,MouseModel,MouseSerial,MouseRemark,MonitorBrand,MonitorModel,MonitorSerial,MonitorRemark,KeyboardBrand,KeyboardModel,KeyboardSerial,KeyboardRemark,ScannerBrand,ScannerModel,ScannerSerial,ScannerRemark,DVDBrand,DVDModel,DVDSerial,DVDRemark,HDD1Brand,HDD1Model,HDD1Serial,HDD1Remark,HDD2Brand,HDD2Model,HDD2Serial,HDD2Remark,PrinterBrand,PrinterModel,PrinterSerial,PrinterRemark,UPSBrand,UPSModel,UPSSerial,UPSRemark,PCICardBrand,PCICardModel,PCICardSerial,PCIRemark,BluetoothBrand,BluetoothModel,BluetoothSerial,BluetoothRemark,HUBBrand,HUBModel,HUBSerial,HUBRemark,OtherHardwareName1,OtherHardwareBrand,OtherHardwareModel,OtherHardwareSerial,OtherHardwareRemark,OtherHardwareName2,OtherHardware1Brand,OtherHardware1Model,OtherHardware1Serial,OtherHardware1Remark,OtherHardwareName3,OtherHardware2Brand,OtherHardware2Model,OtherHardware2Serial,OtherHardware2Remark,FixAccess,CreateBy,UpdateBy,DateCreate,DateUpdate,PlantID,DepartmentID,LocationID,PhaseID,MachineID,PlantName,DepartmentName,LocationName,PhaseName,MachineName,Zip7Version,Zip7Remark,AcrobatVersion,AcrobatRemark,CureGraphVersion,CureGraphRemark,JP1Version,JP1Remark,MSOfficeVersion,MSOfficeRemark,OracleVersion,OracleRemark,RaidVersion,RaidRemark,SeedWincsVersion,SeedWincsRemark,SharedCPCVersion,SharedCPCRemark,SharedDPanelPCVersion,SharedDPanelPCRemark,TightVNCVersion,TightVNCRemark,TrendMicroVersion,TrendMicroRemark,WindowsVersion,WindowsRemark,OtherSoftware,OtherSoftwareRemark")] PMSheet pmsheet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pmsheet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pmsheet);
        }

        // GET: /MachineSheet/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PMSheet pmsheet = db.PMSheets.Find(id);
            if (pmsheet == null)
            {
                return HttpNotFound();
            }
            return View(pmsheet);
        }

        // POST: /MachineSheet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PMSheet pmsheet = db.PMSheets.Find(id);
            db.PMSheets.Remove(pmsheet);
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
