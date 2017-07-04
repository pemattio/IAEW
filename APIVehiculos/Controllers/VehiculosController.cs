using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using APIVehiculos.WCF;
using SitioWeb.Models;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using APIVehiculos.Models;

namespace APIVehiculos.Controllers
{
   
    public class VehiculosController : ApiController
    {
        private TuricorEntities db = new TuricorEntities();

        //[Route("api/Vehiculos/Paises/{access_token}")]
        [Route("api/Vehiculos/Paises/")]
        [HttpGet]
        //public IHttpActionResult Paises(string access_token)
        public IHttpActionResult Paises()
        {
            try{
            //if (Validar(access_token) == true)
            //{
            var cliente = new WCF.WCFReservaVehiculosClient();
            ConsultarPaisesResponse Paises = cliente.ConsultarPaises();
           return Ok(Paises.Paises);
            //}
            //return Unauthorized();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            } 
        }
               
        [Route("api/Vehiculos/Ciudades/{IdPais}")]
        [HttpGet]
        public IHttpActionResult Ciudades(int IdPais)
        {
            try { 
            var cliente = new WCF.WCFReservaVehiculosClient();
            var pais = new ConsultarCiudadesRequest { IdPais=IdPais }; 
            ConsultarCiudadesResponse Ciudad = cliente.ConsultarCiudades(pais);
            return Ok(Ciudad.Ciudades);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            } 
        }

        [Route("api/Vehiculos/VehiculosDisponibles/{IdCiudad}")]
        [HttpGet]
        public IHttpActionResult VehiculosDisponibles(int IdCiudad)
        {
            try { 
            var cliente = new WCF.WCFReservaVehiculosClient();
            var Ciudad =  new ConsultarVehiculosRequest { IdCiudad = IdCiudad};
            ConsultarVehiculosDisponiblesResponse VehiculosDisponibles = cliente.ConsultarVehiculosDisponibles(Ciudad);
            return Ok(VehiculosDisponibles.VehiculosEncontrados);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            } 
        }

        [Route("api/Vehiculos/Cliente")]
        [HttpGet]
        public IHttpActionResult Cliente()
        {
            try
            {
                return Ok(db.Cliente);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [Route("api/Vehiculos/Vendedor")]
        [HttpGet]
        public IHttpActionResult Vendedor()
        {
            try
            {
                return Ok(db.Vendedor);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

 private Boolean Validar(string token)
        {
            TokenInfo resultado = new TokenInfo();
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("http://104.197.29.243:8080");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage Res = client.GetAsync("/openam/oauth2/tokeninfo").Result;
                if (Res.IsSuccessStatusCode)
                {
                    var TokenResponse = Res.Content.ReadAsStringAsync().Result;
                    resultado = JsonConvert.DeserializeObject<TokenInfo>((TokenResponse));

                }
                if (token == resultado.access_token) { return true; } else { return false; }
               
            }
        }
    }
}
