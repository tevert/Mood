using Mood.Util;
using System.Web.Mvc;

namespace Mood
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ErrorHandler.AiHandleErrorAttribute());
            filters.Add(new JsonHandlerAttribute());
        }
    }
}
