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
using System.Text;


namespace SitioWeb.Controllers
{
    public class VehiculoController : Controller
    {
        string Baseurl = "http://localhost:61319/";

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Vehiculo/       
        public ActionResult Index(int? idPais, int? idCiudad)
        {
            try
            {
                if (idPais != null)
                {
                    //@ViewBag.paisSelec = idPais;
                    ViewBag.Pais = new SelectList(ConsultarPaises(), "Id", "Nombre", idPais);
                    if (idCiudad != null) { ViewBag.Ciudad = new SelectList(ConsultarCiudades(idPais), "Id", "Nombre", idCiudad); }
                    else { ViewBag.Ciudad = new SelectList(ConsultarCiudades(idPais), "Id", "Nombre"); }
                }
                else
                {
                    ViewBag.Pais = new SelectList(ConsultarPaises(), "Id", "Nombre");
                }
                if (idCiudad != null)
                {
                    List<Vehiculo> list = ConsultarVehiculos(idCiudad);
                    Session["vehiculos"] = list;
                    Session["idPais"] = idPais;
                    return View(list);
                }
                else
                {
                    return View();
                }
            }
            catch { 
                return RedirectToAction("NoAutorizado", "Home"); }
            
        }

        public ActionResult Reservar(int Id)
        {
            ViewBag.Cliente = ConsultarCliente();
            ViewBag.Vendedor = ConsultarVendedor();
            List<Vehiculo> list = (List<Vehiculo>)Session["vehiculos"];
            Vehiculo vehiculo = list.Single(a => a.Id ==Id);
            return View(vehiculo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearReserva(FormCollection form)
        {
            List<Vehiculo> list = (List<Vehiculo>)Session["vehiculos"];
            Vehiculo vehiculo = list.Single(a => a.Id == Convert.ToInt32(form["Id"]));
            Reserva reserva = new Reserva
            { 
                IdCliente = Convert.ToInt32(form["Clientes"]),
                IdVendedor = Convert.ToInt32(form["Vendedores"]),
                IdVehiculoCiudad = vehiculo.VehiculoCiudadId,
                 FechaReserva= DateTime.Now.ToString(),
                Costo = Convert.ToDecimal(vehiculo.PrecioPorDia),
                FechaHoraDevolucion=form["FechaHoraDevolucion"],
                FechaHoraRetiro = form["FechaHoraRetiro"],
                LugarRetiro = form["LugarRetiro"],
                LugarDevolucion = form["LugarDevolucion"],
                 PrecioVenta = Convert.ToDecimal(vehiculo.CalculoPrecioAlPublico()),
                 IdPais = (int)Session["idPais"],
                 IdCiudad = vehiculo.CiudadId,
                Id = Convert.ToInt32(form["Id"]),
            };
            if(!GuardarReserva(reserva))
            {
                return View();
            }
            return RedirectToAction("Index", "Reserva"); ;

        }

        private Boolean GuardarReserva(Reserva reserva)
        {
                var jsonRequest = JsonConvert.SerializeObject(reserva);

                var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var cliente = new HttpClient()
                {
                    BaseAddress = new Uri(Baseurl)
                };

                var url = "api/Reservas/ReservarVehiculo";

                var res = cliente.PostAsync(url, httpContent).Result;

                if (!res.IsSuccessStatusCode)
                {
                    return false;

                }
                return true;
        }

        private List<Vendedor> ConsultarVendedor()
        {
            List<Vendedor> list = new List<Vendedor>();

            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = client.GetAsync("api/Vehiculos/Vendedor").Result;
                if (Res.IsSuccessStatusCode)
                {
                    var VendedorResponse = Res.Content.ReadAsStringAsync().Result;
                    list = JsonConvert.DeserializeObject<List<Vendedor>>((VendedorResponse));

                }

                return list;
            }
        }

        private List<Cliente> ConsultarCliente()
        {
            List<Cliente> list = new List<Cliente>();

            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = client.GetAsync("api/Vehiculos/Cliente").Result;
                if (Res.IsSuccessStatusCode)
                {
                    var ClienteResponse = Res.Content.ReadAsStringAsync().Result;
                    list = JsonConvert.DeserializeObject<List<Cliente>>((ClienteResponse));

                }

                return list;
            }
        }

        public List<Pais> ConsultarPaises() 
        {

            List<Pais> list = new List<Pais>();
            
            using (var client = new HttpClient())
            {
                AccessToken token = (AccessToken)Session["token"];
               
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = client.GetAsync("api/Vehiculos/Paises/"+token.access_token ).Result;
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
