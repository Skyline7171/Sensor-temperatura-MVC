using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MonitorSerial.Models;
using System.Data.Entity;

namespace MonitorSerial.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new MonitorModelConnection())
            {
                var ultimaLectura = db.Lectura
                    .Include(l => l.Dispositivo)
                    .OrderByDescending(l => l.FechaHora)
                    .FirstOrDefault();

                return View(ultimaLectura);
            }
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