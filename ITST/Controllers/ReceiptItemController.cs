using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ITST.Models;

namespace ITST.Controllers
{
    public class ReceiptItemController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        // GET: /ReceiptItem/
        public ActionResult Index()
        {
            return View(db.BillReceiptLists.ToList());
        }

        // GET: /ReceiptItem/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
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

        // GET: /ReceiptItem/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /ReceiptItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ListID,DeviceID,CreateBy,UpdateBy,DateCreate,DateUpdate,SerialNumber,Cause,Type,Model,Brand,Plant,Department,Location,Phase,MachineName,UserName,DeviceName,InRepairDate,BillReceiptType,BillReceiptNo,CompanyName")] BillReceiptList billreceiptlist)
        {
            if (ModelState.IsValid)
            {
                db.BillReceiptLists.Add(billreceiptlist);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(billreceiptlist);
        }

        // GET: /ReceiptItem/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
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

        // POST: /ReceiptItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ListID,DeviceID,CreateBy,UpdateBy,DateCreate,DateUpdate,SerialNumber,Cause,Type,Model,Brand,Plant,Department,Location,Phase,MachineName,UserName,DeviceName,InRepairDate,BillReceiptType,BillReceiptNo,CompanyName")] BillReceiptList billreceiptlist)
        {
            if (ModelState.IsValid)
            {
                db.Entry(billreceiptlist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(billreceiptlist);
        }

        // GET: /ReceiptItem/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
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

        // POST: /ReceiptItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BillReceiptList billreceiptlist = db.BillReceiptLists.Find(id);
            db.BillReceiptLists.Remove(billreceiptlist);
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
