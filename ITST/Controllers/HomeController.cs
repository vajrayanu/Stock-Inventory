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
    public class HomeController : Controller
    {
        private ITStockEntities1 db = new ITStockEntities1();

        public ActionResult Index()
        {
            ViewBag.ScannerRepairQty = db.Devices.Where(d => d.DeviceTypeID == 58 && d.StatusID == 2).Count();
            ViewBag.PanelPCRepairQty = db.Devices.Where(d => d.DeviceTypeID == 50 && d.StatusID == 2).Count();
            ViewBag.PrinterRepairQty = db.Devices.Where(d => d.DeviceTypeID == 56 && d.StatusID == 2).Count();
            ViewBag.PresetPCRepairQty = db.Devices.Where(d => d.DeviceTypeID == 54 && d.StatusID == 2).Count();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public JsonResult FindDevice(string prefixText)
        {
            var modelname = from x in db.Devices
                            where x.SerialNumber.StartsWith(prefixText) || x.DeviceType.Type.StartsWith(prefixText) || x.Brand.BrandName.StartsWith(prefixText) || x.Model.ModelName.StartsWith(prefixText)
                            select new
                            {
                                value = x.DeviceType.Type + " " + x.Model.ModelName + " " + x.Brand.BrandName + " " + x.SerialNumber,
                                name = x.SerialNumber,
                                id = x.DeviceID
                            };
            var result = Json(modelname.Take(5).ToList());
            ViewBag.ModelID = new SelectList(db.Models.OrderBy(d => d.ModelName), "ModelID", "ModelName");
            return result;
        }

        public static IEnumerable<SelectListItem> getMenu()
        {
            IList<SelectListItem> types = new List<SelectListItem>
            {
                new SelectListItem() {Text="", Value=""},
                new SelectListItem() { Text="Change LocationStock", Value="I"},
                new SelectListItem() { Text="Emergency", Value="E"},
            };
            return types;
        }
    }
}