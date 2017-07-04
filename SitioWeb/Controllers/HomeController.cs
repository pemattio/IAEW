using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SitioWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace SitioWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
                return View();
        }

        public ActionResult callBack(string code, string scope, string state)
        {
            Session["token"] = SolicitarAccesToken(code);
            return View();
            
        }
        private AccessToken SolicitarAccesToken(string code)
        {
            AccessToken resultado = new AccessToken();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://104.197.29.243:8080");

                var form = new Dictionary<string, string>  
               {  
                   {"grant_type", "authorization_code"},  
                   {"client_id", "TPI_GrupoNro5"},                     
                   {"client_secret","pass12345" },
                   {"code",code },
                   {"redirect_uri","http://localhost:54705/home/callBack"}
               };

                var content = new FormUrlEncodedContent(form);

                HttpResponseMessage Res = client.PostAsync("/openam/oauth2/access_token", content).Result;

                if (Res.IsSuccessStatusCode)
                {
                    var TokenResponse = Res.Content.ReadAsStringAsync().Result;
                    resultado = JsonConvert.DeserializeObject<AccessToken>((TokenResponse));

                }

                return resultado;
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult NoAutorizado()
        {
            ViewBag.Message = "No estás autorizado a ingresar a la página que intentaste acceder. Por favor inicie sesión.";

            return View();
        }
    }
}