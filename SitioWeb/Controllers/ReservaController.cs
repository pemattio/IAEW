using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
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
            try
            {

                List<Reserva> list = new List<Reserva>();

                using (var client = new HttpClient())
                {
                    AccessToken token = (AccessToken)Session["token"];
                    if (token == null)
                    {
                        token = new AccessToken();
                        token.access_token = "noAutorizado";
                    }
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = client.GetAsync("api/Reservas/ListadoDeReservasTuricor/"+ token.access_token ).Result;
                    if (Res.IsSuccessStatusCode)
                    {
                        var ReservaResponse = Res.Content.ReadAsStringAsync().Result;
                        list = JsonConvert.DeserializeObject<List<Reserva>>(ReservaResponse);

                    }
                    else
                    {
                        throw new HttpRequestException("Código: " + (int)Res.StatusCode + ". Descripción: " + Res.ReasonPhrase);
                    }
                    return View(list);
                }
            }
            catch (HttpRequestException hre) { return RedirectToAction("Error", "Home", new { mensaje = hre.Message }); }
            catch (Exception ex) { return RedirectToAction("Error", "Home", new { mensaje = ex.Message }); }
        }

        public ActionResult Todas()
        {
            try
            {

                List<Reserva> list = new List<Reserva>();

                using (var client = new HttpClient())
                {
                    AccessToken token = (AccessToken)Session["token"];
                    if (token == null)
                    {
                        token = new AccessToken();
                        token.access_token = "noAutorizado";
                    }
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = client.GetAsync("api/Reservas/ListadoDeReservas/" + token.access_token).Result;
                    if (Res.IsSuccessStatusCode)
                    {
                        var ReservaResponse = Res.Content.ReadAsStringAsync().Result;
                        list = JsonConvert.DeserializeObject<List<Reserva>>(ReservaResponse);

                    }
                    else
                    {
                        throw new HttpRequestException("Código: " + (int)Res.StatusCode + ". Descripción: " + Res.ReasonPhrase);
                    }
                    return View(list);
                }
            }
            catch (HttpRequestException hre) { return RedirectToAction("Error", "Home", new { mensaje = hre.Message }); }
            catch (Exception ex) { return RedirectToAction("Error", "Home", new { mensaje = ex.Message }); }
        }

        public ActionResult Cancelar(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reserva Reserva = new Reserva();

            using (var client = new HttpClient())
            {
                AccessToken token = (AccessToken)Session["token"];
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = client.GetAsync("api/Reserva/CancelarReserva/" + id+"/"+token.access_token).Result;
                if (Res.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Reserva");

                }
                return RedirectToAction("Index", "Home");
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
