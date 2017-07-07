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
            catch (Exception ex) { return RedirectToAction("Error", "Home", new { mensaje = ex.Message }); }

        }

        public ActionResult Reservar(int Id)
        {
            try
            {
                ViewBag.Cliente = ConsultarCliente();
                ViewBag.Vendedor = ConsultarVendedor();
                List<Vehiculo> list = (List<Vehiculo>)Session["vehiculos"];
                Vehiculo vehiculo = list.Single(a => a.Id == Id);
                return View(vehiculo);
            }
            catch (Exception ex) { return RedirectToAction("Error", "Home", new { mensaje = ex.Message }); }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearReserva(FormCollection form)
        {
            try
            {
                List<Vehiculo> list = (List<Vehiculo>)Session["vehiculos"];
                Vehiculo vehiculo = list.Single(a => a.Id == Convert.ToInt32(form["Id"]));
                Reserva reserva = new Reserva
                {
                    IdCliente = Convert.ToInt32(form["Clientes"]),
                    IdVendedor = Convert.ToInt32(form["Vendedores"]),
                    IdVehiculoCiudad = vehiculo.VehiculoCiudadId,
                    FechaReserva = DateTime.Now.ToString(),
                    Costo = Convert.ToDecimal(vehiculo.PrecioPorDia),
                    FechaHoraDevolucion = form["FechaHoraDevolucion"],
                    FechaHoraRetiro = form["FechaHoraRetiro"],
                    LugarRetiro = form["LugarRetiro"],
                    LugarDevolucion = form["LugarDevolucion"],
                    PrecioVenta = Convert.ToDecimal(vehiculo.CalculoPrecioAlPublico()),
                    IdPais = (int)Session["idPais"],
                    IdCiudad = vehiculo.CiudadId,
                    Id = Convert.ToInt32(form["Id"]),
                };
                if (!GuardarReserva(reserva))
                {
                    //return RedirectToAction("Reservar", new { Id = Convert.ToInt32(form["Id"]) });
                    return View();
                }
                return RedirectToAction("Index", "Reserva"); ;
            }
            catch (Exception ex) { return RedirectToAction("Error", "Home", new { mensaje = ex.Message }); }
        }

        private Boolean GuardarReserva(Reserva reserva)
        {
            var jsonRequest = JsonConvert.SerializeObject(reserva);

            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var cliente = new HttpClient()
            {
                BaseAddress = new Uri(Baseurl)
            };
            try
            {
                AccessToken token = (AccessToken)Session["token"];
                if (token == null)
                {
                    token = new AccessToken();
                    token.access_token = "noAutorizado";
                }
                var url = "api/Reservas/ReservarVehiculo/" + token.access_token;

                var res = cliente.PostAsync(url, httpContent).Result;

                if (!res.IsSuccessStatusCode)
                {
                    System.Web.Http.HttpError error = res.Content.ReadAsAsync<System.Web.Http.HttpError>().Result;
                    if (error != null)
                        throw new HttpRequestException("Código: " + (int)res.StatusCode + ". Descripción: " + res.ReasonPhrase + ". Detalle: " + error.ExceptionMessage);
                    else
                        throw new HttpRequestException("Código: " + (int)res.StatusCode + ". Descripción: " + res.ReasonPhrase);
                }
                return true;
            }
            catch (HttpRequestException hre) { throw hre; }
            catch (Exception ex) { throw ex; }
        }

        private List<Vendedor> ConsultarVendedor()
        {
            List<Vendedor> list = new List<Vendedor>();

            using (var client = new HttpClient())
            {
                try
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
                    HttpResponseMessage Res = client.GetAsync("api/Vehiculos/Vendedor/" + token.access_token).Result;
                    if (Res.IsSuccessStatusCode)
                    {
                        var VendedorResponse = Res.Content.ReadAsStringAsync().Result;
                        list = JsonConvert.DeserializeObject<List<Vendedor>>((VendedorResponse));

                    }
                    else
                    {
                        System.Web.Http.HttpError error = Res.Content.ReadAsAsync<System.Web.Http.HttpError>().Result;
                        if (error != null)
                            throw new HttpRequestException("Código: " + (int)Res.StatusCode + ". Descripción: " + Res.ReasonPhrase + ". Detalle: " + error.ExceptionMessage);
                        else
                            throw new HttpRequestException("Código: " + (int)Res.StatusCode + ". Descripción: " + Res.ReasonPhrase);
                    }
                    return list;
                }
                catch (HttpRequestException hre) { throw hre; }
                catch (Exception ex) { throw ex; }
            }
        }

        private List<Cliente> ConsultarCliente()
        {
            List<Cliente> list = new List<Cliente>();

            using (var client = new HttpClient())
            {
                try
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
                    HttpResponseMessage Res = client.GetAsync("api/Vehiculos/Cliente/" + token.access_token).Result;
                    if (Res.IsSuccessStatusCode)
                    {
                        var ClienteResponse = Res.Content.ReadAsStringAsync().Result;
                        list = JsonConvert.DeserializeObject<List<Cliente>>((ClienteResponse));

                    }
                    else
                    {
                        System.Web.Http.HttpError error = Res.Content.ReadAsAsync<System.Web.Http.HttpError>().Result;
                        if (error != null)
                            throw new HttpRequestException("Código: " + (int)Res.StatusCode + ". Descripción: " + Res.ReasonPhrase + ". Detalle: " + error.ExceptionMessage);
                        else
                            throw new HttpRequestException("Código: " + (int)Res.StatusCode + ". Descripción: " + Res.ReasonPhrase);
                    }
                    return list;
                }
                catch (HttpRequestException hre) { throw hre; }
                catch (Exception ex) { throw ex; }
            }
        }

        public List<Pais> ConsultarPaises()
        {
            List<Pais> list = new List<Pais>();
            using (var client = new HttpClient())
            {
                try
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
                    HttpResponseMessage Res = client.GetAsync("api/Vehiculos/Paises/" + token.access_token).Result;
                    if (Res.IsSuccessStatusCode)
                    {
                        var PaisResponse = Res.Content.ReadAsStringAsync().Result;
                        list = JsonConvert.DeserializeObject<List<Pais>>((PaisResponse));
                    }
                    else
                    {
                        System.Web.Http.HttpError error = Res.Content.ReadAsAsync<System.Web.Http.HttpError>().Result;
                        if (error != null)
                            throw new HttpRequestException("Código: " + (int)Res.StatusCode + ". Descripción: " + Res.ReasonPhrase + ". Detalle: " + error.ExceptionMessage);
                        else
                            throw new HttpRequestException("Código: " + (int)Res.StatusCode + ". Descripción: " + Res.ReasonPhrase);
                    }
                    return list;
                }
                catch (HttpRequestException hre) { throw hre; }
                catch (Exception ex) { throw ex; }
            }
        }

        public List<Ciudad> ConsultarCiudades(int? idPais)
        {

            List<Ciudad> list = new List<Ciudad>();

            using (var client = new HttpClient())
            {
                try
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
                    HttpResponseMessage Res = client.GetAsync("api/Vehiculos/Ciudades/" + idPais + "/" + token.access_token).Result;
                    if (Res.IsSuccessStatusCode)
                    {
                        var CiudadResponse = Res.Content.ReadAsStringAsync().Result;
                        list = JsonConvert.DeserializeObject<List<Ciudad>>((CiudadResponse));
                    }
                    else
                    {
                        System.Web.Http.HttpError error = Res.Content.ReadAsAsync<System.Web.Http.HttpError>().Result;
                        if (error != null)
                            throw new HttpRequestException("Código: " + (int)Res.StatusCode + ". Descripción: " + Res.ReasonPhrase + ". Detalle: " + error.ExceptionMessage);
                        else
                            throw new HttpRequestException("Código: " + (int)Res.StatusCode + ". Descripción: " + Res.ReasonPhrase);
                    }
                    return list;
                }
                catch (HttpRequestException hre) { throw hre; }
                catch (Exception ex) { throw ex; }
            }
        }
        public List<Vehiculo> ConsultarVehiculos(int? idCiudad)
        {

            List<Vehiculo> list = new List<Vehiculo>();

            using (var client = new HttpClient())
            {
                try
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
                    HttpResponseMessage Res = client.GetAsync("api/Vehiculos/VehiculosDisponibles/" + idCiudad + "/" + token.access_token).Result;
                    if (Res.IsSuccessStatusCode)
                    {
                        var VehiculosResponse = Res.Content.ReadAsStringAsync().Result;
                        list = JsonConvert.DeserializeObject<List<Vehiculo>>((VehiculosResponse));
                    }
                    else
                    {
                        System.Web.Http.HttpError error = Res.Content.ReadAsAsync<System.Web.Http.HttpError>().Result;
                        if (error != null)
                            throw new HttpRequestException("Código: " + (int)Res.StatusCode + ". Descripción: " + Res.ReasonPhrase + ". Detalle: " + error.ExceptionMessage);
                        else
                            throw new HttpRequestException("Código: " + (int)Res.StatusCode + ". Descripción: " + Res.ReasonPhrase);
                    }

                    return list;
                }
                catch (HttpRequestException hre) { throw hre; }
                catch (Exception ex) { throw ex; }
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
