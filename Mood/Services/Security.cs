using System;
using System.Web;

namespace Mood.Services
{
    public class Security : ISecurity
    {
        public bool IsAuthenticated
        {
            get
            {
                return HttpContext.Current.Request.IsAuthenticated;
            }
        }

        public string UserName
        {
            get
            {
                return HttpContext.Current.User.Identity.Name;
            }
        }
    }
}