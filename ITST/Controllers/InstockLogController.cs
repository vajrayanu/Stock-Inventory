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

namespace ITST.Controllers
{
    public class InstockLogController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();
        //
        // GET: /InstockLog/
        public ActionResult Index()
        {
            var recordinstock = db.RecordInstocks.OrderByDescending(r=>r.DateInstock);
            return View(recordinstock.ToList());
        }
	}
}