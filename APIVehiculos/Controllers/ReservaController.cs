using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using APIVehiculos.WCF;
using APIVehiculos.Models;
using SitioWeb.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace APIVehiculos.Controllers
{

    public class ReservaController : ApiController
    {
        private TuricorEntities db = new TuricorEntities();

        [Route("api/Reservas/ListadoDeReservas/{access_token}")]
        [HttpGet]
        public IHttpActionResult ListadoDeReservas(string access_token)
        {
            try
            {
                if (Validar(access_token) == true)
                {
                    var cliente = new WCF.WCFReservaVehiculosClient();
                    ConsultarReservasRequest res = new ConsultarReservasRequest { IncluirCanceladas = true };
                    ConsultarReservasResponse Reservas = cliente.ConsultarReservas(res);
                    return Ok(Reservas.Reservas);
                } return InternalServerError();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/Reservas/ListadoDeReservasCanceladas")]
        [HttpGet]
        public IHttpActionResult ListadoDeReservasCanceladas()
        {

            try
            {
                var cliente = new WCF.WCFReservaVehiculosClient();
                ConsultarReservasRequest res = new ConsultarReservasRequest { IncluirCanceladas = true };
                ConsultarReservasResponse Reservas = cliente.ConsultarReservas(res);
                return Ok(Reservas.Reservas);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
        [Route("api/Reserva/CancelarReserva/{idReserva}/{access_token}")]
        [HttpGet]
       
        public IHttpActionResult cancelarReservas(string idReserva,string access_token)
        {

            try
            {
                if (Validar(access_token) == true)
                {
           
                    var cliente = new WCF.WCFReservaVehiculosClient();
                    CancelarReservaRequest res = new CancelarReservaRequest {CodigoReserva = idReserva };
                    CancelarReservaResponse Reservas = cliente.CancelarReserva(res);
                    if (db.Reserva.Find(idReserva)!=null)
                    {
                    db.Reserva.Remove(db.Reserva.Single(a => a.CodigoReserva == idReserva));
                    db.SaveChanges();
            }
                }
        return Ok();
            }
            catch (Exception ex)
            {
                ex = new FaultException("Error 41: La reserva que se desea cancelar no existe.");
                return InternalServerError(ex);
            }
        }

        [Route("api/Reservas/ReservarVehiculo")]
        [HttpPost]
        public IHttpActionResult ReservarVehiculo(Reserva reserva)
        {
            try
            {
                if (reserva == null)
                    return BadRequest();
                
                var cliente = new WCF.WCFReservaVehiculosClient();
                ReservarVehiculoRequest res = new ReservarVehiculoRequest
                {
                    ApellidoNombreCliente = db.Cliente.Single(a => a.Id == reserva.IdCliente).Apellido + " " + db.Cliente.Single(a => a.Id == reserva.IdCliente).Nombre,
                    FechaHoraDevolucion = Convert.ToDateTime(reserva.FechaHoraDevolucion),
                    FechaHoraRetiro = Convert.ToDateTime(reserva.FechaHoraRetiro),
                    IdVehiculoCiudad = reserva.IdVehiculoCiudad,
                    LugarRetiro = lugarRetiroDevolucion("1"),
                    LugarDevolucion = lugarRetiroDevolucion("1"),
                    NroDocumentoCliente = db.Cliente.Single(a => a.Id == reserva.IdCliente).NroDocumento,
                    
                };
                ReservarVehiculoResponse Reservas = cliente.ReservarVehiculo(res);
                reserva.CodigoReserva = Reservas.Reserva.CodigoReserva;
                db.Reserva.Add(reserva);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private LugarRetiroDevolucion lugarRetiroDevolucion(string lugarRetiroDevolucion)
        {
            switch (lugarRetiroDevolucion)
            {
                case "0":
                    return LugarRetiroDevolucion.Aeropuerto;
                case "1":
                    return LugarRetiroDevolucion.TerminalBuses;
                case "2":
                    return LugarRetiroDevolucion.Hotel;
                default:
                    return new LugarRetiroDevolucion();
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


