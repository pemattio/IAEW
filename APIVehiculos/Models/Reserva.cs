//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace APIVehiculos.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Reserva
    {
        public int Id { get; set; }
        public string CodigoReserva { get; set; }
        public string FechaReserva { get; set; }
        public string FechaHoraRetiro { get; set; }
        public string FechaHoraDevolucion { get; set; }
        public int IdCliente { get; set; }
        public int IdVendedor { get; set; }
        public decimal Costo { get; set; }
        public decimal PrecioVenta { get; set; }
        public int IdVehiculoCiudad { get; set; }
        public int IdCiudad { get; set; }
        public int IdPais { get; set; }
    
        public virtual Cliente Cliente { get; set; }
        public virtual Vendedor Vendedor { get; set; }
    }
}
