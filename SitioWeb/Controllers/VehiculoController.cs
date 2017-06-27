using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SitioWeb.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using SitioWeb;

namespace SitioWeb.Controllers
{
    public class VehiculoController : Controller
    {
        string Baseurl = "http://localhost:61319/";

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Vehiculo/       
        public ActionResult Index(int? idPais, int? idCiudad)
        {
            ViewBag.Paises = ConsultarPaises();
            if (idPais != null){
                @ViewBag.paisSelec = idPais;
                ViewBag.Ciudades = ConsultarCiudades(idPais);
            }
            if (idCiudad != null)
                {
                    List<Vehiculo> list = ConsultarVehiculos(idCiudad);
                return View(list);
                } else 
                {
                return View();
                }
            
        }

        public ActionResult Reservar(int idVehiculo) 
        {

            return View();
        }



        public List<Pais> ConsultarPaises() 
        {

            List<Pais> list = new List<Pais>();
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = client.GetAsync("api/Vehiculos/Paises").Result;
                if (Res.IsSuccessStatusCode)
                {
                    var PaisResponse = Res.Content.ReadAsStringAsync().Result;
                    list = JsonConvert.DeserializeObject<List<Pais>>((PaisResponse));
                    
                }

                return list;
            }
        }
        public List<Ciudad> ConsultarCiudades(int? idPais)
        {

            List<Ciudad> list = new List<Ciudad>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = client.GetAsync("api/Vehiculos/Ciudades/"+idPais).Result;
                if (Res.IsSuccessStatusCode)
                {
                    var CiudadResponse = Res.Content.ReadAsStringAsync().Result;
                    list = JsonConvert.DeserializeObject<List<Ciudad>>((CiudadResponse));

                }

                return list;
            }
        }
        public List<Vehiculo> ConsultarVehiculos(int? idCiudad)
        {

            List<Vehiculo> list = new List<Vehiculo>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = client.GetAsync("api/Vehiculos/VehiculosDisponibles/"+idCiudad).Result;
                if (Res.IsSuccessStatusCode)
                {
                    var VehiculosResponse = Res.Content.ReadAsStringAsync().Result;
                    list = JsonConvert.DeserializeObject<List<Vehiculo>>((VehiculosResponse));

                }

                return list;
            }
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
