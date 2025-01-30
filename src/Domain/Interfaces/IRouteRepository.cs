using Domain.Entities;

namespace Domain.Interfaces;

public interface IRouteRepository
{
    void AddRoute(Route route);
    List<Route> GetRoutes();
}
