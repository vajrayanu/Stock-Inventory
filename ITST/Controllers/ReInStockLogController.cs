using ITST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITST.Controllers
{
    public class ReInStockLogController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();
        //
        // GET: /ReInStockLog/
        public ActionResult Index()
        {
            var recordreinstock = db.RecordReinstocks.OrderByDescending(r=>r.DateRequest);
            return View(recordreinstock.ToList());
        }
	}
}