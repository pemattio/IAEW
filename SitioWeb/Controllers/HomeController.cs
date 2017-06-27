using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Constants;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using SitioWeb.Models;

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
            AccessToken token=SolicitarAccessToken(code);
            return View();
            
        }

        private AccessToken SolicitarAccessToken(string code)
        {
            //string resultado = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://104.197.29.243:8080/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var contenido = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", Clients.Client1.Id),
                new KeyValuePair<string, string>("client_secret", Clients.Client1.Secret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", Paths.AuthorizeCodeCallBackPath)
            });
                HttpResponseMessage Res = client.PostAsync("openam/oauth2/access_token", contenido).Result;
                AccessToken token=new AccessToken();
                if (Res.IsSuccessStatusCode)
                {
                    var AccessTokenResponse = Res.Content.ReadAsStringAsync().Result;
                    token = JsonConvert.DeserializeObject<AccessToken>((AccessTokenResponse));

                }

                return token;
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
    }
}