using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using APIVehiculos.WCF;

namespace APIVehiculos.Controllers
{
    
    public class VehiculosController : ApiController
    {
        [Route("api/Vehiculos/Paises")]
        [HttpGet]
        public PaisEntity[] Paises()
        {
           
            var cliente = new WCF.WCFReservaVehiculosClient();
            ConsultarPaisesResponse Paises = cliente.ConsultarPaises();
            return Paises.Paises;
        }
        [Route("api/Vehiculos/Ciudades/{IdPais}")]
        [HttpGet]
        public CiudadEntity[] Ciudades(int IdPais)
        {

            var cliente = new WCF.WCFReservaVehiculosClient();
            var pais = new ConsultarCiudadesRequest { IdPais=IdPais }; 
            ConsultarCiudadesResponse Ciudad = cliente.ConsultarCiudades(pais);
            return Ciudad.Ciudades;
        }

        [Route("api/Vehiculos/VehiculosDisponibles/{IdCiudad}")]
        [HttpGet]
        public VehiculoModel[] VehiculosDisponibles(int IdCiudad)
        {

            var cliente = new WCF.WCFReservaVehiculosClient();
            var Ciudad =  new ConsultarVehiculosRequest { IdCiudad = IdCiudad};
            ConsultarVehiculosDisponiblesResponse VehiculosDisponibles = cliente.ConsultarVehiculosDisponibles(Ciudad);
            return VehiculosDisponibles.VehiculosEncontrados;
        }

    }
}
