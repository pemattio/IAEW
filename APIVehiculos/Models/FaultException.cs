using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIVehiculos.Models
{
    public class FaultException : Exception
    {
        public int codigo { get; set; }
        public string descripcion { get; set; }

        public override string ToString()
        {
            return "Error " + codigo + ": " + descripcion;
        }

        public FaultException(string mensaje) : base(mensaje)
        {
        }
    }
}