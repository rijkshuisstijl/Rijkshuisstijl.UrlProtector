using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Rijkshuisstijl.UrlProtector {
    public class Routes : IRouteProvider {

        
        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                new RouteDescriptor {
                    Route = new Route("Admin/UrlProtector/{action}",
                                      new RouteValueDictionary {
                                          {"area", "Rijkshuisstijl.UrlProtector"},
                                          {"controller", "UrlProtector"}
                                      }, new RouteValueDictionary(),
                                      new RouteValueDictionary {{"area", "Rijkshuisstijl.UrlProtector"}},
                                      new MvcRouteHandler())
                }
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var route in GetRoutes()) {
                routes.Add(route);
            }
        }
    }
}