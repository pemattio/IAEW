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
                ConsultarReservasRequest res = new ConsultarReservasRequest { IncluirCanceladas = false};
                ConsultarReservasResponse Reservas = cliente.ConsultarReservas(res);
                List<Reserva> reservas = new List<Reserva>();
                foreach (ReservaEntity r in Reservas.Reservas)
                {
                    Reserva aux = new Reserva
                    {
                        CodigoReserva = r.CodigoReserva,
                        Estado = estadoReserva(r.Estado),
                        FechaCancelacion=Convert.ToString(r.FechaCancelacion),
                        FechaHoraRetiro=Convert.ToString(r.FechaHoraRetiro),
                        FechaHoraDevolucion=Convert.ToString(r.FechaHoraDevolucion),
                        FechaReserva=Convert.ToString(r.FechaReserva),
                        LugarDevolucion=r.LugarDevolucion,
                        LugarRetiro=r.LugarRetiro,
                        NroDocumentoCliente=r.NroDocumentoCliente,
                        TotalReserva=r.TotalReserva,
                        IdVehiculoCiudad=r.VehiculoPorCiudadId
                    };
                    reservas.Add(aux);
                }
                return Ok(reservas);
                } return Unauthorized();

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/Reservas/ListadoDeReservasTuricor/{access_token}")]
        [HttpGet]
        public IHttpActionResult ListadoDeReservasTuricor(string access_token)
        {
            try
            {
                if (Validar(access_token) == true)
                {
                return Ok(db.Reserva);
                } return Unauthorized();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/Reservas/ListadoDeReservasCanceladas")]
        [HttpGet]
        public IHttpActionResult ListadoDeReservasNoCanceladas()
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

        public IHttpActionResult cancelarReservas(string idReserva, string access_token)
        {

            try
            {
                if (Validar(access_token) == true)
                {

                    var cliente = new WCF.WCFReservaVehiculosClient();
                    CancelarReservaRequest res = new CancelarReservaRequest { CodigoReserva = idReserva };
                    CancelarReservaResponse Reservas = cliente.CancelarReserva(res);
                    if (db.Reserva.FirstOrDefault(a => a.CodigoReserva == idReserva) != null)
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
                    LugarRetiro = lugarRetiroDevolucion(reserva.LugarRetiro),
                    LugarDevolucion = lugarRetiroDevolucion(reserva.LugarDevolucion),
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
                case "Aeropuerto":
                    return LugarRetiroDevolucion.Aeropuerto;
                case "TerminalBuses":
                    return LugarRetiroDevolucion.TerminalBuses;
                case "Hotel":
                    return LugarRetiroDevolucion.Hotel;
                default:
                    return new LugarRetiroDevolucion();
            }
        }

        private string estadoReserva(EstadoReservaEnum estadoReserva)
        {
            switch (estadoReserva)
            {
                case (EstadoReservaEnum.Activa):
                    return "Activa";
                case (EstadoReservaEnum.Cancelada):
                    return "Cancelada";
                default:
                    return "";
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


