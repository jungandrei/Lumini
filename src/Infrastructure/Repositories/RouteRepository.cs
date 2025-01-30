using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories;

public class RouteRepository : IRouteRepository
{
    private const string FilePath = @"C:\Users\User\Downloads\routes.txt";

    public void AddRoute(Route route)
    {
        var line = $"{route.Origin},{route.Destination},{route.Cost}{Environment.NewLine}";
        File.AppendAllText(FilePath, line);
    }

    public List<Route> GetRoutes()
    {
        if(!File.Exists(FilePath)) return [];       

        var routes = File.ReadAllLines(FilePath)
        .Skip(1)
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(line =>
        {
            var parts = line.Split(',');
            
            if (parts.Length != 3 || !int.TryParse(parts[2], out int cost))
            {
                Console.WriteLine($"Erro ao processar linha: {line}");
                return null;
            }

            return new Route(parts[0], parts[1], cost);
        })
        .Where(route => route != null)
        .ToList();

        return routes!;
    }
}