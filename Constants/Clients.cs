using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Constants
{
    public static class Clients
    {
        public readonly static Client Client1 = new Client
        {
            Id = "TPI_GrupoNro5",
            Secret = "pass12345",
            RedirectUrl = Paths.AuthorizeCodeCallBackPath
        };

       
    }

    public class Client
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public string RedirectUrl { get; set; }
    }
}