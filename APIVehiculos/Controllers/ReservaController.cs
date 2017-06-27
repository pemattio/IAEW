using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using APIVehiculos.WCF;

namespace APIVehiculos.Controllers
{
    public class ReservaController : ApiController
    {

        [Route("api/Reservas/ListadoDeReservas")]
        [HttpGet]
        public ReservaEntity[] ListadoDeReservas()
        {

            var cliente = new WCF.WCFReservaVehiculosClient();
            ConsultarReservasRequest res = new ConsultarReservasRequest { IncluirCanceladas = false };
            ConsultarReservasResponse Reservas = cliente.ConsultarReservas(res);
            return Reservas.Reservas;
        }

        [Route("api/Reservas/ListadoDeReservasCanceladas")]
        [HttpGet]
        public ReservaEntity[] ListadoDeReservasCanceladas()
        {

            var cliente = new WCF.WCFReservaVehiculosClient();
            ConsultarReservasRequest res = new ConsultarReservasRequest { IncluirCanceladas = true };
            ConsultarReservasResponse Reservas = cliente.ConsultarReservas(res);
            return Reservas.Reservas;
        }

        [Route("api/Reserva/CancelarReserva/{idReserva}")]
        [HttpGet]
        public void CancelarReserva(string idReserva)
        {

            var cliente = new WCF.WCFReservaVehiculosClient();
            CancelarReservaRequest res = new CancelarReservaRequest { CodigoReserva = idReserva };
            CancelarReservaResponse Reservas = cliente.CancelarReserva(res);

        }

        [Route("api/Reserva/ReservarVehiculo")]
        [HttpGet]
        public void ReservarVehiculo([FromBody]ReservaEntity reserva)
        {

            var cliente = new WCF.WCFReservaVehiculosClient();
            ReservarVehiculoRequest res = new ReservarVehiculoRequest { 
                ApellidoNombreCliente = reserva.ApellidoNombreCliente, 
                FechaHoraDevolucion = reserva.FechaHoraDevolucion,
                FechaHoraRetiro = reserva.FechaHoraRetiro,
                IdVehiculoCiudad = reserva.VehiculoPorCiudadId,
                LugarDevolucion = lugarRetiroDevolucion(reserva.LugarDevolucion),
                LugarRetiro = lugarRetiroDevolucion(reserva.LugarRetiro),
                NroDocumentoCliente = reserva.NroDocumentoCliente
            };
            ReservarVehiculoResponse Reservas = cliente.ReservarVehiculo(res);

        }

        private LugarRetiroDevolucion lugarRetiroDevolucion(string lugarRetiroDevolucion)
        {
            switch (lugarRetiroDevolucion)
            {
                case "0":
                    return LugarRetiroDevolucion.Aeropuerto;
                case "1":
                    return LugarRetiroDevolucion.Hotel;
                case "2":
                    return LugarRetiroDevolucion.TerminalBuses;
                default:
                    return new LugarRetiroDevolucion();
            }
        }
    }
}
