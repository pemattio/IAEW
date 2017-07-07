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

        [Route("api/Vehiculos/Paises/{access_token}")]
        [HttpGet]
        public IHttpActionResult Paises(string access_token)
        {
            try
            {
                if (Validar(access_token) == true)
                {
                    var cliente = new WCF.WCFReservaVehiculosClient();
                    ConsultarPaisesResponse Paises = cliente.ConsultarPaises();
                    return Ok(Paises.Paises);
                }
                return Unauthorized();
            }
            catch (System.ServiceModel.FaultException<APIVehiculos.WCF.StatusResponse> ex) {
                FaultException fe;
                fe = new FaultException("Código: " + ex.Detail.ErrorCode.ToString() + ". Descripción: " + ex.Detail.ErrorDescription.ToString());
                fe.codigo = Convert.ToInt32(ex.Detail.ErrorCode.ToString());
                fe.descripcion = ex.Detail.ErrorDescription.ToString();
                return InternalServerError(fe);
            }
            catch (Exception ex){
                ex = new Exception(ex.Message, ex);
                return InternalServerError(ex);} 
        }

        [Route("api/Vehiculos/Ciudades/{IdPais}/{access_token}")]
        [HttpGet]
        public IHttpActionResult Ciudades(int IdPais, string access_token)
        {
            try
            {
                if (Validar(access_token) == true)
                {
                    var cliente = new WCF.WCFReservaVehiculosClient();
                    var pais = new ConsultarCiudadesRequest { IdPais = IdPais };
                    ConsultarCiudadesResponse Ciudad = cliente.ConsultarCiudades(pais);
                    return Ok(Ciudad.Ciudades);
                } 
                return Unauthorized();
            }
            catch (System.ServiceModel.FaultException<APIVehiculos.WCF.StatusResponse> ex){
                FaultException fe;
                fe = new FaultException("Código: " + ex.Detail.ErrorCode.ToString() + ". Descripción: " + ex.Detail.ErrorDescription.ToString());
                fe.codigo = Convert.ToInt32(ex.Detail.ErrorCode.ToString());
                fe.descripcion = ex.Detail.ErrorDescription.ToString();
                return InternalServerError(fe);
            }
            catch (Exception ex)
            {
                ex = new Exception(ex.Message, ex);
                return InternalServerError(ex);
            }
        }

        [Route("api/Vehiculos/VehiculosDisponibles/{IdCiudad}/{access_token}")]
        [HttpGet]
        public IHttpActionResult VehiculosDisponibles(int IdCiudad, string access_token)
        {
            try
            {
                if (Validar(access_token) == true)
                {
                    var cliente = new WCF.WCFReservaVehiculosClient();
                    var Ciudad = new ConsultarVehiculosRequest { IdCiudad = IdCiudad };
                    ConsultarVehiculosDisponiblesResponse VehiculosDisponibles = cliente.ConsultarVehiculosDisponibles(Ciudad);
                    return Ok(VehiculosDisponibles.VehiculosEncontrados);
                }
                return Unauthorized();
            }
            catch (System.ServiceModel.FaultException<APIVehiculos.WCF.StatusResponse> ex) 
            {
                FaultException fe;
                fe = new FaultException("Código: " + ex.Detail.ErrorCode.ToString() + ". Descripción: " + ex.Detail.ErrorDescription.ToString());
                fe.codigo = Convert.ToInt32(ex.Detail.ErrorCode.ToString());
                fe.descripcion = ex.Detail.ErrorDescription.ToString();
                return InternalServerError(fe);
            }
            catch (Exception ex)
            {
                ex = new Exception(ex.Message, ex);
                return InternalServerError(ex);
            }
        }

        [Route("api/Vehiculos/Cliente/{access_token}")]
        [HttpGet]
        public IHttpActionResult Cliente(string access_token)
        {
            try
            {
                if (Validar(access_token) == true)
                {
                    return Ok(db.Cliente);
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                ex = new Exception(ex.Message, ex);
                return InternalServerError(ex);
            }
        }
        [Route("api/Vehiculos/Vendedor/{access_token}")]
        [HttpGet]
        public IHttpActionResult Vendedor(string access_token)
        {
            try
            {
                if (Validar(access_token) == true)
                {
                    return Ok(db.Vendedor);
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                ex = new Exception(ex.Message, ex);
                return InternalServerError(ex);
            }
        }

        private Boolean Validar(string token)
        {
            TokenInfo resultado = new TokenInfo();
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("http://130.211.183.120:8080");
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
