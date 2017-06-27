using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Api.Models;
using SitioWeb.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;



namespace SitioWeb.Controllers
{
    public class ReservaController : Controller
    {
        string Baseurl = "http://localhost:61319/";
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Reserva/
        public ActionResult Index()
        {
          
            List<Reserva> list = new List<Reserva>();  
              
            using (var client = new HttpClient())  
            {  
                client.BaseAddress = new Uri(Baseurl);  
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = client.GetAsync("api/Reservas/ListadoDeReservas").Result;  
                if (Res.IsSuccessStatusCode)  
                {  
                    var ReservaResponse = Res.Content.ReadAsStringAsync().Result;  
                    list = JsonConvert.DeserializeObject<List<Reserva>>(ReservaResponse);  
  
                }  
                
                return View(list);  
            }  
        }


        public ActionResult Cancelar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reserva Reserva = new Reserva();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = client.GetAsync("api/Reserva/CancelarReserva" + id).Result;
                if (Res.IsSuccessStatusCode)
                {
                    var ReservaResponse = Res.Content.ReadAsStringAsync().Result;
                    Reserva = JsonConvert.DeserializeObject<Reserva>(ReservaResponse);

                }
                if (Reserva == null)
                {
                    return HttpNotFound();
                }
                return View(Reserva);
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
