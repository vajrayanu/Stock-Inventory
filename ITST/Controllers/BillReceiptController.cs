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

namespace ITST.Controllers
{
    public class BillReceiptController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /BillReceipt/
        [Authorize]
        public ActionResult Index()
        {
            return View(db.BillReceipts.ToList());
        }

        // GET: /BillReceipt/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BillReceipt billreceipt = db.BillReceipts.Find(id);
            if (billreceipt == null)
            {
                return HttpNotFound();
            }
            return View(billreceipt);
        }

        // GET: /BillReceipt/Create
        [Authorize]
        public ActionResult Create()
        {
            int length = 4;
            string number = db.BillReceipts.OrderByDescending(b => b.DateCreate).Select(b => b.BillReceiptNo).FirstOrDefault();
            string index = number.Substring(1, 4);
            int value = Int32.Parse(index);
            int ind = value+1;
            //string asString = "RS" + ind.ToString("D" + length);
            string asString = ind.ToString("D" + length);
            Billing billing = new Billing();
            billing.DateCreate = DateTime.Now;
            billing.DateUpdate = DateTime.Now;
            billing.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            billing.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            billing.BillReceiptNo = asString;
            billing.IsPrint = 0;
            return View(billing);
        }

        // POST: /BillReceipt/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BillReceiptID,Type,BillReceiptNo,CompanyName,IsPrint,DateCreate,DateUpdate,CreateBy,UpdateBy")] Billing billing)
        {
            if (ModelState.IsValid)
            {
                BillReceipt billreceipt = new BillReceipt();
                billreceipt.Type = billing.Type;
                billreceipt.CompanyName = billing.CompanyName;
                billreceipt.IsPrint = billing.IsPrint;
                billreceipt.DateCreate = billing.DateCreate;
                billreceipt.DateUpdate = billing.DateUpdate;
                if (billing.Type == "Repair")
                {
                    billreceipt.BillReceiptNo = "R"+billing.BillReceiptNo;
                }
                else if (billing.Type == "Sale")
                {
                    billreceipt.BillReceiptNo = "S" + billing.BillReceiptNo;
                }
                else if (billing.Type == "Destroy")
                {
                    billreceipt.BillReceiptNo = "D" + billing.BillReceiptNo;
                }
                //billreceipt.BillReceiptNo = billing.BillReceiptNo;
                billreceipt.CreateBy = billing.CreateBy;
                billreceipt.UpdateBy = billing.UpdateBy;
                db.BillReceipts.Add(billreceipt);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(billing);
        }

        public ActionResult setSentRepairSelected(string[] ids)
        {
            if (ModelState.IsValid)
            {
                int[] id = null;
                if (ids != null)
                {
                    id = new int[ids.Length];
                    int j = 0;
                    foreach (string i in ids)
                    {
                        int.TryParse(i, out id[j++]);
                    }
                }

                if (id != null && id.Length > 0)
                {
                    {
                        var allSelected = db.Devices.Where(a => id.Contains(a.DeviceID)).ToList();

                        foreach (var i in allSelected)
                        {
                            if(i.StatusID != 6)
                            {
                                var c = db.Devices.Where(b => b.DeviceID.Equals(i.DeviceID)).FirstOrDefault();
                                {
                                    c.StatusID = 6;
                                    c.StatusName = "Sent Repair";
                                    c.DateUpdate = DateTime.Now;
                                    c.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;

                                    RecordInRepair recordinrepair = new RecordInRepair();
                                    recordinrepair.Brand = db.Brands.Where(b => b.BrandID == c.BrandID).Select(b => b.BrandName).DefaultIfEmpty().First();
                                    recordinrepair.DeviceType = db.DeviceTypes.Where(b => b.DeviceTypeID == c.DeviceTypeID).Select(b => b.Type).DefaultIfEmpty().First();
                                    recordinrepair.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                                    recordinrepair.DateRequest = c.DateUpdate;
                                    recordinrepair.Cause = "Sent Repair";
                                    recordinrepair.Model = db.Models.Where(b => b.ModelID == c.ModelID).Select(b => b.ModelName).DefaultIfEmpty().First();
                                    recordinrepair.SerialNumber = db.Devices.Where(b => b.DeviceID == c.DeviceID).Select(b => b.SerialNumber).DefaultIfEmpty().First();
                                    recordinrepair.Plant = db.Plants.Where(b => b.PlantID == c.PlantID).Select(b => b.PlantName).DefaultIfEmpty().First();
                                    recordinrepair.Department = db.Departments.Where(b => b.DepartmentID == c.DepartmentID).Select(b => b.DepartmentName).DefaultIfEmpty().First();
                                    recordinrepair.Location = db.Locations.Where(b => b.LocationID == c.LocationID).Select(b => b.LocationName).DefaultIfEmpty().First();
                                    recordinrepair.Phase = db.Phases.Where(b => b.PhaseID == c.PhaseID).Select(b => b.PhaseName).DefaultIfEmpty().First();
                                    recordinrepair.LocationStock = db.LocationStocks.Where(b => b.LocationName == c.LocationStockName).Select(b => b.LocationName).DefaultIfEmpty().First();
                                    recordinrepair.Machine = db.Machines.Where(b => b.MachineID == c.MachineID).Select(b => b.MachineName).DefaultIfEmpty().First();
                                    recordinrepair.Status = db.Status.Where(b => b.StatusID == c.StatusID).Select(b => b.Status1).DefaultIfEmpty().First();
                                    recordinrepair.UserName = db.Users.Where(b => b.UserID == c.UserID).Select(b => b.FullName).DefaultIfEmpty().First();
                                    if (c.IsAsset == true)
                                    {
                                        recordinrepair.IsFixAsset = "Asset";
                                    }
                                    db.Entry(c).State = EntityState.Modified;
                                    db.RecordInRepairs.Add(recordinrepair);
                                }
                            }
                            else
                            {
                                return RedirectToAction("Index", "BillReceipt");
                            }
                        }
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("SendRepair", "Device");
            }
            return RedirectToAction("Index", "BillReceipt");
        }


        [Authorize]
        public ActionResult AddItem(int?id)
        {
            string number = db.BillReceipts.Where(b => b.BillReceiptID == id).Select(b => b.BillReceiptNo).DefaultIfEmpty().First();
            string type = number.Substring(0, 1);

            BillReceiptList billreceiptlist = new BillReceiptList();
            billreceiptlist.BillReceiptNo = db.BillReceipts.Where(b => b.BillReceiptID == id).Select(b => b.BillReceiptNo).DefaultIfEmpty().First();
            billreceiptlist.CreateBy = System.Web.HttpContext.Current.User.Identity.Name;
            billreceiptlist.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            billreceiptlist.DateCreate = DateTime.Now;
            billreceiptlist.DateUpdate = DateTime.Now;
            billreceiptlist.Cause = "Scrap";

            if (type == "D")
            {
                billreceiptlist.BillReceiptType = "Destroy";
            }
            else if(type == "S")
            {
                billreceiptlist.BillReceiptType = "Sale";
            }

            //billreceiptlist.BillReceiptType = "Sale";
            billreceiptlist.CompanyName = db.BillReceipts.Where(b => b.BillReceiptID == id).Select(b => b.CompanyName).DefaultIfEmpty().First();
            billreceiptlist.Plant = "PCLT";
            billreceiptlist.Department = "Safety & Environment";
            billreceiptlist.Location = "Scrap PCLT";
            billreceiptlist.Phase = "2";
            billreceiptlist.Unit = 1;
            return View(billreceiptlist);
        }

        // POST: /BillReceipt/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddItem([Bind(Include = "DeviceID,CreateBy,UpdateBy,DateCreate,DateUpdate,SerialNumber,Cause,Type,Model,Brand,Plant,Department,Location,Phase,MachineName,UserName,DeviceName,InRepairDate,BillReceiptType,BillReceiptNo,CompanyName,Price,Unit")] BillReceiptList billreceiptlist)
        {
            if (string.IsNullOrEmpty(billreceiptlist.Type))
            {
                ModelState.AddModelError("Type", "Type is Required");
            }

            if (ModelState.IsValid)
            {
                db.BillReceiptLists.Add(billreceiptlist);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(billreceiptlist);
        }

        [Authorize]
        public ActionResult SetPriceSale(string id)
        {
            if(User.Identity.Name != "rp15082" && User.Identity.Name != "rp13074")
            {
                return Content("No Permission");
            }
            var receipt = db.BillReceiptLists.Where(b => b.BillReceiptNo == id);
            ViewBag.ReceiptNo = id;
            return View(receipt.ToList());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetPriceSale(List<BillReceiptList> billreceiptlists)
        {
            foreach (var d in billreceiptlists)
            {
                var c = db.BillReceiptLists.Where(a => a.ListID.Equals(d.ListID)).FirstOrDefault();
                var e = db.Devices.Where(x => x.DeviceID == d.DeviceID).FirstOrDefault();
                {
                    if (c.Price != billreceiptlists[0].Price && d.BillReceiptType == "Sale" && e != null)
                      //if (c.Price != d.Price && d.BillReceiptType == "Sale" && e != null)
                    {
                        RecordSale recordsale = new RecordSale();
                        recordsale.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                        recordsale.DateRequest = DateTime.Now;
                        recordsale.Cause = db.RecordInRepairs.Where(r => r.SerialNumber == c.SerialNumber).Select(r => r.Cause).DefaultIfEmpty().First();
                        recordsale.DeviceType = db.DeviceTypes.Where(t => t.Type == c.Type).Select(t => t.Type).DefaultIfEmpty().First();
                        recordsale.Model = db.Models.Where(m => m.ModelName == c.Model).Select(t => t.ModelName).DefaultIfEmpty().First();
                        recordsale.Brand = db.Brands.Where(t => t.BrandName == c.Brand).Select(t => t.BrandName).DefaultIfEmpty().First();
                        recordsale.SerialNumber = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.SerialNumber).DefaultIfEmpty().First();
                        recordsale.Status = "Sent Sale";
                        recordsale.Plant = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.Plant.PlantName).DefaultIfEmpty().First();
                        recordsale.Department = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.Department.DepartmentName).DefaultIfEmpty().First();
                        recordsale.Location = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.Location.LocationName).DefaultIfEmpty().First();
                        recordsale.Phase = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.PhaseName).DefaultIfEmpty().First();
                        recordsale.LocationStock = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.LocationStockName).DefaultIfEmpty().First();
                        recordsale.Machine = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.Machine.MachineName).DefaultIfEmpty().First();
                        recordsale.UserName = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.User.FullName).DefaultIfEmpty().First();
                        recordsale.DeviceName = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.DeviceName).DefaultIfEmpty().First();
                        db.RecordSales.Add(recordsale);
                    }
                    else if (c.Price != billreceiptlists[0].Price && d.BillReceiptType == "Sale" && e == null)
                    //else if (c.Price != d.Price && d.BillReceiptType == "Sale" && e == null)
                    {
                        RecordSale recordsale = new RecordSale();
                        recordsale.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                        recordsale.DateRequest = DateTime.Now;
                        recordsale.Cause = "Remove scrap";
                        recordsale.DeviceType = "Scrap";
                        recordsale.Status = "Sent Sale";
                        recordsale.Plant = "PCLT";
                        recordsale.Department = "Safety & Environment";
                        recordsale.Location = "Scrap PCLT";
                        recordsale.Phase = "2";
                        db.RecordSales.Add(recordsale);
                    }
                      if (d.BillReceiptType == "Sale" && e != null)
                      {
                          c.Price = billreceiptlists[0].Price;
                          e.StatusID = 7;
                          e.StatusName = "Sent Sale";
                          e.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                          e.DateUpdate = DateTime.Now;
                          db.Entry(c).State = EntityState.Modified;
                          db.Entry(e).State = EntityState.Modified;
                      }
                      else if (d.BillReceiptType == "Sale" && e == null)
                      {
                          c.Price = billreceiptlists[0].Price;
                          db.Entry(c).State = EntityState.Modified;
                      }
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public  ActionResult SetPrice (string id)
        {
            var receipt = db.BillReceiptLists.Where(b => b.BillReceiptNo == id);
            ViewBag.ReceiptNo = id;
            return View(receipt.ToList());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetPrice(List<BillReceiptList> billreceiptlists)
        {
            foreach (var d in billreceiptlists)
            {
                var c = db.BillReceiptLists.Where(a => a.ListID.Equals(d.ListID)).FirstOrDefault();
                var e = db.Devices.Where(x => x.DeviceID == d.DeviceID).FirstOrDefault();
                {
                    if (c.Price != d.Price && d.BillReceiptType == "Repair" && e.StatusID == 6 || e.StatusID == 8)
                    {
                        RecordInRepair recordinrepair = new RecordInRepair();
                        recordinrepair.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                        recordinrepair.DateRequest = DateTime.Now;
                        recordinrepair.Cause = db.RecordInRepairs.Where(r => r.SerialNumber == c.SerialNumber).Select(r => r.Cause).DefaultIfEmpty().First();
                        recordinrepair.DeviceType = db.DeviceTypes.Where(t => t.Type == c.Type).Select(t => t.Type).DefaultIfEmpty().First();
                        recordinrepair.Model = db.Models.Where(m => m.ModelName == c.Model).Select(t => t.ModelName).DefaultIfEmpty().First();
                        recordinrepair.Brand = db.Brands.Where(t => t.BrandName == c.Brand).Select(t => t.BrandName).DefaultIfEmpty().First();
                        recordinrepair.SerialNumber = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.SerialNumber).DefaultIfEmpty().First();
                        recordinrepair.Status = "Sent Repair";
                        recordinrepair.Plant = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.Plant.PlantName).DefaultIfEmpty().First();
                        recordinrepair.Department = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.Department.DepartmentName).DefaultIfEmpty().First();
                        recordinrepair.Location = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.Location.LocationName).DefaultIfEmpty().First();
                        recordinrepair.Phase = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.PhaseName).DefaultIfEmpty().First();
                        recordinrepair.LocationStock = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.LocationStockName).DefaultIfEmpty().First();
                        recordinrepair.Machine = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.Machine.MachineName).DefaultIfEmpty().First();
                        recordinrepair.UserName = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.User.FullName).DefaultIfEmpty().First();
                        recordinrepair.DeviceName = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.DeviceName).DefaultIfEmpty().First();
                        db.RecordInRepairs.Add(recordinrepair);
                    }
                    else if (c.Price != d.Price && d.BillReceiptType == "Sale" && e != null)
                    {
                        RecordSale recordsale = new RecordSale();
                        recordsale.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                        recordsale.DateRequest = DateTime.Now;
                        recordsale.Cause = db.RecordInRepairs.Where(r => r.SerialNumber == c.SerialNumber).Select(r => r.Cause).DefaultIfEmpty().First();
                        recordsale.DeviceType = db.DeviceTypes.Where(t => t.Type == c.Type).Select(t => t.Type).DefaultIfEmpty().First();
                        recordsale.Model = db.Models.Where(m => m.ModelName == c.Model).Select(t => t.ModelName).DefaultIfEmpty().First();
                        recordsale.Brand = db.Brands.Where(t => t.BrandName == c.Brand).Select(t => t.BrandName).DefaultIfEmpty().First();
                        recordsale.SerialNumber = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.SerialNumber).DefaultIfEmpty().First();
                        recordsale.Status = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.StatusName).DefaultIfEmpty().First();
                        recordsale.Plant = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.Plant.PlantName).DefaultIfEmpty().First();
                        recordsale.Department = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.Department.DepartmentName).DefaultIfEmpty().First();
                        recordsale.Location = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.Location.LocationName).DefaultIfEmpty().First();
                        recordsale.Phase = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.PhaseName).DefaultIfEmpty().First();
                        recordsale.LocationStock = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.LocationStockName).DefaultIfEmpty().First();
                        recordsale.Machine = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.Machine.MachineName).DefaultIfEmpty().First();
                        recordsale.UserName = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.User.FullName).DefaultIfEmpty().First();
                        recordsale.DeviceName = db.Devices.Where(t => t.DeviceID == c.DeviceID).Select(t => t.DeviceName).DefaultIfEmpty().First();
                        db.RecordSales.Add(recordsale);
                    }
                    else if (c.Price != d.Price && d.BillReceiptType == "Sale" && e == null)
                    {
                        RecordSale recordsale = new RecordSale();
                        recordsale.RequestBy = System.Web.HttpContext.Current.User.Identity.Name;
                        recordsale.DateRequest = DateTime.Now;
                        recordsale.Cause = "Remove scrap";
                        recordsale.DeviceType = "Scrap";
                        recordsale.Status = "Sent Sale";
                        recordsale.Plant = "PCLT";
                        recordsale.Department = "Safety & Environment";
                        recordsale.Location = "Scrap PCLT";
                        recordsale.Phase = "2";
                        db.RecordSales.Add(recordsale);
                    }
                    if (c.Price != d.Price && d.BillReceiptType == "Repair" && e.StatusID == 6 || e.StatusID == 8)
                    {
                        c.Price = d.Price;
                        e.StatusID = 6;
                        e.StatusName = "Sent Repair";
                        e.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                        e.DateUpdate = DateTime.Now;
                        db.Entry(c).State = EntityState.Modified;
                        db.Entry(e).State = EntityState.Modified;
                    } else if(c.Price != d.Price && d.BillReceiptType == "Sale" && e != null)
                    {
                        c.Price = d.Price;
                        e.StatusID = 7;
                        e.StatusName = "Sent Sale";
                        e.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                        e.DateUpdate = DateTime.Now;
                        db.Entry(c).State = EntityState.Modified;
                        db.Entry(e).State = EntityState.Modified;
                    }
                        else if(d.BillReceiptType == "Sale" && e != null)
                    {
                        c.Price = d.Price;
                        e.StatusID = 7;
                        e.StatusName = "Sent Sale";
                        e.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                        e.DateUpdate = DateTime.Now;
                        db.Entry(c).State = EntityState.Modified;
                        db.Entry(e).State = EntityState.Modified;
                    }
                    else if (c.Price != d.Price && e == null)
                    {
                        c.Price = d.Price;
                        db.Entry(c).State = EntityState.Modified;
                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ViewList (string id)
        {
            var receipt = db.BillReceiptLists.Where(b => b.BillReceiptNo == id);
            ViewBag.ReceiptNo = id;
            return View(receipt.ToList());
        }

        public ActionResult ViewListSale(string id)
        {
            var receipt = db.BillReceiptLists.Where(b => b.BillReceiptNo == id);
            ViewBag.ReceiptNo = id;
            return View(receipt.ToList());
        }

        public ActionResult ExportListSale(string id)
        {
            var receipts = from receipt in db.BillReceiptLists where receipt.BillReceiptNo == id group receipt by receipt.Type into dv let m = dv.FirstOrDefault() select m;
            var totalitem = db.BillReceiptLists.Where(b => b.BillReceiptNo == id);

            var billID = db.BillReceipts.Where(b => b.BillReceiptNo == id).Select(b => b.BillReceiptID).DefaultIfEmpty().First();

            BillReceipt billreceipt = db.BillReceipts.Find(billID);
            billreceipt.IsPrint = 1;
            db.Entry(billreceipt).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.ReceiptNo = id;
            ViewBag.TotalItem = totalitem.Count();
            return new Rotativa.ActionAsPdf("ExportListSalePdf", new { id });

        }

        public ActionResult ExportListSalePdf(string id)
        {
            var receipts = from receipt in db.BillReceiptLists where receipt.BillReceiptNo == id group receipt by receipt.Type into dv let m = dv.FirstOrDefault() select m;
            var totalitem = db.BillReceiptLists.Where(b => b.BillReceiptNo == id).Sum(t=>t.Unit);
            string type = id.Substring(0, 1);
            ViewBag.ReceiptNo = id;
            ViewBag.Type = type;
            ViewBag.TotalItem = totalitem;
            return View(receipts);
        }

        [Authorize]
        public ActionResult PrintList(string id, int?lid)
        {
            if (ModelState.IsValid)
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Reports"), "SentRepairListReport.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }

                List<BillReceiptList> cm = new List<BillReceiptList>();
                cm = db.BillReceiptLists.Where(d => d.BillReceiptNo == id).ToList();

                BillReceipt billreceipt = db.BillReceipts.Find(lid);
                billreceipt.IsPrint = 1;
                db.Entry(billreceipt).State = EntityState.Modified;
                db.SaveChanges();

                ReportDataSource rd = new ReportDataSource("DataSet3", cm);
                lr.DataSources.Add(rd);
                string reportType = "pdf";
                string mimeType;
                string encoding;
                string fileNameExtension;



                string deviceInfo =

                "<DeviceInfo>" +
                "  <OutputFormat>" + "Pdf" + "</OutputFormat>" +
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

        [Authorize]
        public ActionResult PrintListSale(string id, int? lid)
        {
            if (ModelState.IsValid)
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/Reports"), "SentSaleListReport.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    return View("Index");
                }

                List<BillReceiptList> cm = new List<BillReceiptList>();
                cm = db.BillReceiptLists.Where(d => d.BillReceiptNo == id).ToList();
                var groupedCustomerList = cm
                    .GroupBy(u => u.Type)
                    .Select(grp => new { Type = grp.Key, cm = grp.ToList() })
                    .ToList();

                BillReceipt billreceipt = db.BillReceipts.Find(lid);
                billreceipt.IsPrint = 1;
                db.Entry(billreceipt).State = EntityState.Modified;
                db.SaveChanges();

                ReportDataSource rd = new ReportDataSource("DataSet1", groupedCustomerList);
                lr.DataSources.Add(rd);
                string reportType = "pdf";
                string mimeType;
                string encoding;
                string fileNameExtension;



                string deviceInfo =

                "<DeviceInfo>" +
                "  <OutputFormat>" + "Pdf" + "</OutputFormat>" +
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

        public static IEnumerable<SelectListItem> ListType()
        {
            IList<SelectListItem> types = new List<SelectListItem>
            {
                new SelectListItem() {Text="", Value=""},
                new SelectListItem() { Text="Repair", Value="Repair"},
                new SelectListItem() { Text="Sale", Value="Sale"},
                new SelectListItem() { Text="Destroy", Value="Destroy"},
            };
            return types;
        }

        public static IEnumerable<SelectListItem> IsPrint()
        {
            IList<SelectListItem> lists = new List<SelectListItem>
            {
                new SelectListItem() {Text="", Value=""},
                new SelectListItem() { Text="Yes", Value="1"},
                new SelectListItem() { Text="No", Value="0"},
            };
            return lists;
        }

        // GET: /BillReceipt/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BillReceipt billreceipt = db.BillReceipts.Find(id);
            billreceipt.DateUpdate = DateTime.Now;
            billreceipt.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
            if (billreceipt == null)
            {
                return HttpNotFound();
            }
            return View(billreceipt);
        }

        // POST: /BillReceipt/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BillReceiptID,Type,BillReceiptNo,CompanyName,IsPrint,DateCreate,DateUpdate,CreateBy,UpdateBy")] BillReceipt billreceipt)
        {
            if (ModelState.IsValid)
            {
                db.Entry(billreceipt).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(billreceipt);
        }

        [Authorize]
        public ActionResult RemoveSaleList(int? id, int? did)
        {
            if (id == null && did == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BillReceiptList billreceiptlist = db.BillReceiptLists.Find(id);
            if (billreceiptlist == null)
            {
                return HttpNotFound();
            }
            return View(billreceiptlist);
        }

        [HttpPost, ActionName("RemoveSaleList")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveSaleList(int id, int? did)
        {
            BillReceiptList billreceiptlist = db.BillReceiptLists.Find(id);
            var bid = billreceiptlist.BillReceiptNo;
            var isprint = db.BillReceipts.Where(b => b.BillReceiptNo == bid).Select(b => b.IsPrint).DefaultIfEmpty().First();
            if (isprint == 0)
            {
                db.BillReceiptLists.Remove(billreceiptlist);
                if (did != null)
                {
                    Device device = db.Devices.Find(did);
                    device.StatusID = 4;
                    device.StatusName = "In Sale";
                    device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                    device.DateUpdate = DateTime.Now;
                }
                db.SaveChanges();
                return RedirectToAction("ViewListSale", "BillReceipt", new { id = billreceiptlist.BillReceiptNo });
            }
            else if (isprint == 1)
            {
                ViewBag.Isprint = isprint;
                return View(billreceiptlist);
            }
            return View(billreceiptlist);
        }

        [Authorize]
        public ActionResult RemoveRepairList(int? id, int? did)
        {
            if (id == null && did == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BillReceiptList billreceiptlist = db.BillReceiptLists.Find(id);
            if (billreceiptlist == null)
            {
                return HttpNotFound();
            }
            return View(billreceiptlist);
        }

        [HttpPost, ActionName("RemoveRepairList")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveRepairList(int id, int? did)
        {
            BillReceiptList billreceiptlist = db.BillReceiptLists.Find(id);
            var bid = billreceiptlist.BillReceiptNo;
            var deviceID = billreceiptlist.DeviceID;

            Device devices = db.Devices.Find(deviceID);
            var isprint = db.BillReceipts.Where(b => b.BillReceiptNo == bid).Select(b => b.IsPrint).DefaultIfEmpty().First();
            if (isprint == 0 && devices.StatusID == 8)
            {
                db.BillReceiptLists.Remove(billreceiptlist);
                if (did != null)
                {
                    Device device = db.Devices.Find(did);
                    device.StatusID = 2;
                    device.StatusName = "In Repair";
                    device.UpdateBy = System.Web.HttpContext.Current.User.Identity.Name;
                    device.DateUpdate = DateTime.Now;
                }
                db.SaveChanges();
                return RedirectToAction("ViewList", "BillReceipt", new { id = billreceiptlist.BillReceiptNo });
            }
            else if (isprint == 1 || devices.StatusID != 8)
            {
                ViewBag.Isprint = isprint;
                ViewBag.Status = devices.StatusID;
                return View(billreceiptlist);
            }
            return View(billreceiptlist);
        }

        // GET: /BillReceipt/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BillReceipt billreceipt = db.BillReceipts.Find(id);
            if (billreceipt == null)
            {
                return HttpNotFound();
            }
            return View(billreceipt);
        }

        // POST: /BillReceipt/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BillReceipt billreceipt = db.BillReceipts.Find(id);
            db.BillReceipts.Remove(billreceipt);
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
