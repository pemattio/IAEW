using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitioWeb.Models
{
    public class Vehiculo
    {
        public int CantidadDisponible { get; set; } 
        public int CantidadPuertas { get; set; } 
        public int CiudadId { get; set; } 
        public int Id { get; set; } 
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public float PrecioPorDia { get; set; }
        public int Puntaje { get; set; }       
        public bool TieneAireAcon { get; set; }         
        public bool TieneDireccion { get; set; }
        public string TipoCambio { get; set; } 
        public int VehiculoCiudadId { get; set; }
        
        public double CalculoPrecioAlPublico()
        {
            return (double)(this.PrecioPorDia * 1.2);
        }

    }
}