using System;
using System.Linq;
using System.Web.Mvc;
using MonitorSerial.Models;
using MonitorSerial.Services;

namespace MonitorSerial.Controllers
{
    [Authorize]
    public class SensorController : Controller
    {
        private static readonly ServicioSerial servicio =
            new ServicioSerial();

        private static readonly object bloqueo = new object();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Conectar(string nombrePuerto)
        {
            lock (bloqueo)
            {
                try
                {
                    servicio.Conectar(nombrePuerto);

                    return Json(new
                    {
                        exito = true,
                        mensaje = "Conectado correctamente al puerto " + nombrePuerto
                    });
                }
                catch (UnauthorizedAccessException)
                {
                    return Json(new
                    {
                        exito = false,
                        mensaje = "El puerto está siendo utilizado por otra aplicación."
                    });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        exito = false,
                        mensaje = "No se pudo conectar: " + ex.Message
                    });
                }
            }
        }

        [HttpPost]
        public JsonResult Desconectar()
        {
            try
            {
                lock (bloqueo)
                {
                    servicio.Dispose();
                }

                return Json(new
                {
                    exito = true,
                    mensaje = "Conexión serial cerrada correctamente."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exito = false,
                    mensaje = "Error al desconectar: " + ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult Leer(int idDispositivo)
        {
            try
            {
                lock (bloqueo)
                {
                    decimal? temperatura = servicio.LeerTemperatura();

                    if (temperatura == null)
                    {
                        return Json(new
                        {
                            exito = false,
                            mensaje = "No se recibió una temperatura válida."
                        });
                    }

                    if (temperatura < -40 || temperatura > 125)
                    {
                        return Json(new
                        {
                            exito = false,
                            mensaje = "La temperatura está fuera del rango permitido."
                        });
                    }

                    using (var db = new MonitorModelConnection())
                    {
                        var dispositivo = db.Dispositivo.Find(idDispositivo);

                        if (dispositivo == null || !dispositivo.Activo)
                        {
                            return Json(new
                            {
                                exito = false,
                                mensaje = "El dispositivo no existe o está inactivo."
                            });
                        }

                        var lectura = new Lectura
                        {
                            IdDispositivo = idDispositivo,
                            Temperatura = temperatura.Value,
                            FechaHora = DateTime.Now
                        };

                        db.Lectura.Add(lectura);
                        db.SaveChanges();
                    }

                    return Json(new
                    {
                        exito = true,
                        temperatura = temperatura.Value,
                        mensaje = "Temperatura registrada correctamente."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exito = false,
                    mensaje = "Error al obtener la temperatura: " + ex.Message
                });
            }
        }

        [HttpGet]
        public JsonResult ObtenerPuertos()
        {
            try
            {
                string[] puertos = servicio.ObtenerPuertosDisponibles();

                return Json(new
                {
                    exito = true,
                    puertos = puertos
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exito = false,
                    mensaje = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ObtenerDispositivos()
        {
            using (var db = new MonitorModelConnection())
            {
                var dispositivos = db.Dispositivo
                    .Where(d => d.Activo)
                    .Select(d => new
                    {
                        d.IdDispositivo,
                        d.Nombre,
                        d.PuertoCOM
                    })
                    .ToList();

                return Json(new
                {
                    exito = true,
                    dispositivos = dispositivos
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}