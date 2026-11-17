using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MonitorSerial.Models;

namespace MonitorSerial.Controllers
{
    [Authorize]
    public class LecturaController : Controller
    {
        private MonitorModelConnection db = new MonitorModelConnection();

        // GET: Lectura
        public ActionResult Index()
        {
            var lectura = db.Lectura
                .Include(l => l.Dispositivo)
                .OrderByDescending(l => l.FechaHora);

            return View(lectura.ToList());
        }

        // GET: Lectura/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lectura lectura = db.Lectura.Find(id);
            if (lectura == null)
            {
                return HttpNotFound();
            }
            return View(lectura);
        }

        // GET: Lectura/Create
        public ActionResult Create()
        {
            ViewBag.IdDispositivo = new SelectList(db.Dispositivo, "IdDispositivo", "Nombre");
            return View();
        }

        // POST: Lectura/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdLectura,IdDispositivo,Temperatura,FechaHora")] Lectura lectura)
        {
            if (ModelState.IsValid)
            {
                db.Lectura.Add(lectura);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdDispositivo = new SelectList(db.Dispositivo, "IdDispositivo", "Nombre", lectura.IdDispositivo);
            return View(lectura);
        }

        // GET: Lectura/Edit/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lectura lectura = db.Lectura.Find(id);
            if (lectura == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdDispositivo = new SelectList(db.Dispositivo, "IdDispositivo", "Nombre", lectura.IdDispositivo);
            return View(lectura);
        }

        // POST: Lectura/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit([Bind(Include = "IdLectura,IdDispositivo,Temperatura,FechaHora")] Lectura lectura)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lectura).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdDispositivo = new SelectList(db.Dispositivo, "IdDispositivo", "Nombre", lectura.IdDispositivo);
            return View(lectura);
        }

        // GET: Lectura/Delete/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lectura lectura = db.Lectura.Find(id);
            if (lectura == null)
            {
                return HttpNotFound();
            }
            return View(lectura);
        }

        // POST: Lectura/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult DeleteConfirmed(int id)
        {
            Lectura lectura = db.Lectura.Find(id);
            db.Lectura.Remove(lectura);
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
