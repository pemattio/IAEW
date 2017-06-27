using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitioWeb.Models
{
    public class Ciudad
    {
        public int Id{ get; set; }
        public string Nombre { get; set; }
        public Pais Pais { get; set; }
    }
}