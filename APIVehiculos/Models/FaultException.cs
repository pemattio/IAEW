using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIVehiculos.Models
{

    public static class FaultExceptions
    {
        public readonly static FaultException FaultException1 = new FaultException
        {
            codigo = 1,
            descripcion = "La ciudad no se encuentra registrada."
        };

        public readonly static FaultException FaultException21 = new FaultException
        {
            codigo = 21,
            descripcion = "El vehículo seleccionado no existe o no se encuentra disponible."
        };

        public readonly static FaultException FaultException41 = new FaultException
        {
            codigo=41,
            descripcion = "La reserva que se desea cancelar no existe o se encuentra cancelada."
        };

        public readonly static FaultException FaultException51 = new FaultException
        {
            codigo = 51,
            descripcion = "No se encontraron ciudades."
        };

        public readonly static FaultException FaultException61 = new FaultException
        {
            codigo = 61,
            descripcion = "No se encontraron países."
        };

        public readonly static FaultException FaultException99 = new FaultException
        {
            codigo = 99,
            descripcion = "Error No controlado."
        };


    }

    public class FaultException: Exception
    {
        public int codigo {get;set;}
        public string descripcion {get;set;}

        public override string ToString()
        {
            return "Error " + codigo + ": " + descripcion;
        }

        public FaultException()
        {
        }
    }
}