using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IsNsfw.ServiceInterface.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IsNsfw.Mvc
{
    public class LinkKeyRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values[routeKey] == null)
                return false;

            var rv = values[routeKey].ToString();

            return KeyValidator.ValidateKey(rv);
        }
    }
}
