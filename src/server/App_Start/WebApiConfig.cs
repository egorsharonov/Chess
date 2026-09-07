using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ChessApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Конфигурация и службы Web API

            // Маршруты Web API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}/{move}",
                defaults: new 
                {
                    id = RouteParameter.Optional, 
                    move = RouteParameter.Optional
                }
            );
        }
    }
}
