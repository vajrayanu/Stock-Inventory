using ITST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITST.Controllers
{
    public class SaleLogController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();
        //
        // GET: /SaleLog/
        public ActionResult Index()
        {
            var recordsale = db.RecordSales.OrderByDescending(s=>s.DateRequest);
            return View(recordsale.ToList());
        }
	}
}