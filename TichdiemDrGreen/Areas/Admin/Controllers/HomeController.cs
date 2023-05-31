using TichdiemDrGreen.Areas.Admin.Data;
using TichdiemDrGreen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TichdiemDrGreen.Areas.Admin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        Entities db = new Entities();
        public ActionResult Index()
        {
            var model = new HomeModel();
            var customer = db.Customers.Count();
            var code = db.CodeGPs.Where(a => a.Status == 1).Count();

            model.Code = code;
            model.Customer = customer;

            var hhtd = db.CodeGPs.Where(a => a.Category == "1");
            //var ktcd = db.CodeGPs.Where(a => a.Category == "X30");
            //var superman = db.CodeGPs.Where(a => a.Category == "SUPERMAN");
            //var vvg = db.CodeGPs.Where(a => a.Category == "VVG");

            var _hhtd = new Nhanhang()
            {
                SoLuongKhachHang = hhtd.Where(a => a.Phone != null).GroupBy(x => x.Phone).Select(g => g.FirstOrDefault()).Count(),
                Tong = hhtd.Where(a => a.Status == 1).Count(),
                Duhanmuc = db.GiftExchanges.Where(a => a.Product == "1").Count(),
                Chuatrathuong = db.GiftExchanges.Where(a => a.Product == "1" && (a.Status == null || a.Status == 0)).Count(),
                Datrathuong = db.GiftExchanges.Where(a => a.Product == "1" && a.Status == 3).Count(),
                Ma = hhtd.Count(),
                Code = "1"
            };
            //var _ktcd = new Nhanhang()
            //{
            //    SoLuongKhachHang = ktcd.Where(a => a.Phone != null).GroupBy(x => x.Phone).Select(g => g.FirstOrDefault()).Count(),
            //    Tong = ktcd.Where(a => a.Status == 1).Count(),
            //    Duhanmuc = db.GiftExchanges.Where(a => a.Product == "X30").Count(),
            //    Chuatrathuong = db.GiftExchanges.Where(a => a.Product == "X30" && (a.Status == null || a.Status == 0)).Count(),
            //    Datrathuong = db.GiftExchanges.Where(a => a.Product == "X30" && a.Status == 3).Count(),
            //    Ma = ktcd.Count(),
            //    Code = "X30"
            //};            

            model.Nhanhang = new List<Nhanhang>() { _hhtd };
            return View(model);
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
    }
}