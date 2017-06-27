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

             [Route("api/Reserva/CancelarReserva/{idReserva}")]
        [HttpGet]
        public void ListadoDeReservas(string idReserva)
        {

            var cliente = new WCF.WCFReservaVehiculosClient();
            CancelarReservaRequest res = new CancelarReservaRequest { CodigoReserva=idReserva};
            CancelarReservaResponse Reservas = cliente.CancelarReserva(res);
            
        }
    }
}
