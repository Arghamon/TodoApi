using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace TodoApi.Ectensions
{
    public static class GeneralExtensions
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            if (httpContext.User == null)
            {
                return string.Empty;
            }

            return httpContext.User.Claims.Single(c => c.Type == "id").Value;
        }
    }
}
