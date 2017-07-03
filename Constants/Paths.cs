using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Constants
{
    public static class Paths
    {
        /// <summary>
        /// AuthorizationServer project should run on this URL
        /// </summary>
        public const string AuthorizationServerBaseAddress = "http://104.197.29.243:8080/openam";
        /// <summary>
        /// ResourceServer project should run on this URL
        /// </summary>
        public const string ResourceServerBaseAddress = "http://localhost:54705";
        /// <summary>
        /// AuthorizationCodeGrant project should be running on this URL.
        /// </summary>
        public const string AuthorizeCodeCallBackPath = "http://localhost:54705/Home/callBack";

        public const string AuthorizePath = "/OAuth/Authorize";
        public const string TokenPath = "/OAuth/Token";
        public const string LoginPath = "/Account/Login";
        public const string LogoutPath = "/Account/Logout";
        public const string MePath = "/api/Me";
    }
}