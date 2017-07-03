using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIVehiculos.Models
{
    public class TokenInfo
    {
    public string access_token { get; set; }
    public string read { get; set; }
    public string grant_type { get; set; }
    public string[] scope { get; set; }
    public string realm { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
    }
}
