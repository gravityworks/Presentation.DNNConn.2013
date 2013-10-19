using DotNetNuke.Web.Api;

namespace DNNService
{
    public class RouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("DNNService", "default", "{controller}/{action}", new[] { "DNNService" });
            mapRouteManager.MapHttpRoute("DNNService", "GetUserByName", "{controller}/{action}/{portalId}/{userId}", new[] { "DNNService" });
            mapRouteManager.MapHttpRoute("DNNService", "GetUser", "{controller}/{action}/{portalId}/{userId}", new[] { "DNNService" });
            mapRouteManager.MapHttpRoute("DNNService", "GetUsersFromIdList", "{controller}/{action}/{portalId}/{userId}", new[] { "DNNService" });
            mapRouteManager.MapHttpRoute("DNNService", "GetUsers", "{controller}/{action}/{portalId}", new[] { "DNNService" });
            mapRouteManager.MapHttpRoute("DNNService", "CreateUser", "{controller}/{action}/{firstName}/{lastName}/{userName}/{email}/{portalId}/{password}", new[] { "DNNService" });
            mapRouteManager.MapHttpRoute("DNNService", "ChangePassword", "{controller}/{action}/{portalId}/{userId}/{password}/{newValue}", new[] { "DNNService" });
            mapRouteManager.MapHttpRoute("DNNService", "ChangeUserName", "{controller}/{action}/{portalId}/{userId}/{password}/{newValue}", new[] { "DNNService" });
        }
    }
}
