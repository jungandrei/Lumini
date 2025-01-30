using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class RouteService(IRouteRepository repository)
{
    private readonly IRouteRepository _repository = repository;

    public void HandleRegisterRoute()
    {
        ShowAllRoutes();

        Console.WriteLine("\nExemplo de entrada: GRU,BRC,10");
        Console.Write("\nDigite a rota no formato Origem,Destino,Valor: ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("\nFormato inválido! Entrada não pode ser vazia.");
            return;
        }

        if (!ValidateRouteInput(input, out string origin, out string destination, out int cost))
        {
            Console.WriteLine("\nFormato inválido! Use o formato Origem,Destino,Valor.");
            return;
        }

        if (!ValidateThreeLetterCode(origin, destination))
            return;

        if (RouteExists(origin, destination))
        {
            Console.WriteLine("\nA rota já existe! Não é possível adicionar duplicadas.");
            return;
        }

        RegisterRoute(origin, destination, cost);
        Console.WriteLine("\nRota registrada com sucesso!");

        ShowAllRoutes();
    }

    public void HandleFindBestRoute()
    {
        var routes = GetAvailableRoutes();
        if (routes == null) return;

        ShowAllRoutes();

        Console.Write("\nDigite a rota no formato Origem-Destino: ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("\nFormato inválido! Entrada não pode ser vazia.");
            return;
        }

        if (!ValidateRouteSearchInput(input, out string origin, out string destination))
        {
            Console.WriteLine("\nFormato inválido! Use o formato Origem-Destino.");
            return;
        }
        
        if (!ValidateRouteExistence(routes, origin, destination))        
            return;        

        var result = FindBestRoute(origin, destination);
        Console.WriteLine($"\nMelhor Rota: {result}");
    }

    public void RegisterRoute(string origin, string destination, int cost)
    {
        var route = new Route(origin, destination, cost);
        _repository.AddRoute(route);
    }

    public string FindBestRoute(string origin, string destination)
    {
        var routes = _repository.GetRoutes();
        var bestPath = new List<string>();

        int bestCost = CalculateCheapestRoute(routes, origin, destination, 0, new List<string>(), ref bestPath);

        return bestCost == int.MaxValue
            ? "Nenhuma rota encontrada."
            : $"{string.Join(" - ", bestPath)} ao custo de ${bestCost}";
    }
    
    private static int CalculateCheapestRoute(
        List<Route> routes,
        string current,
        string destination,
        int currentCost,
        List<string> path,
        ref List<string> bestPath)
    {        
        if (path.Contains(current))
            return int.MaxValue;

        path.Add(current);
        
        if (current == destination)
        {
            bestPath = new List<string>(path);
            return currentCost;
        }

        var possibleRoutes = routes.Where(r => r.Origin == current).ToList();
        if (possibleRoutes.Count == 0) return int.MaxValue;

        int bestCost = int.MaxValue;

        foreach (var route in possibleRoutes)
        {
            var tempPath = new List<string>(path);
            var candidatePath = new List<string>();

            int candidateCost = CalculateCheapestRoute(routes, route.Destination, destination, currentCost + route.Cost, tempPath, ref candidatePath);

            if (candidateCost < bestCost)
            {
                bestCost = candidateCost;
                bestPath = candidatePath;
            }
        }

        return bestCost;
    }

    private void ShowAllRoutes()
    {
        var routes = _repository.GetRoutes();
        Console.WriteLine("\nRotas Disponíveis:");
        foreach (var route in routes)
        {
            Console.WriteLine($"{route.Origin} -> {route.Destination}, Custo: ${route.Cost}");
        }
    }

    private static bool ValidateRouteInput(string input, out string origin, out string destination, out int cost)
    {
        origin = destination = string.Empty;
        cost = 0;

        var parts = input?.Split(',');
        if (parts == null || parts.Length != 3 || !int.TryParse(parts[2], out cost))
            return false;

        if (!IsValidCity(parts[0]) || !IsValidCity(parts[1]))
            return false;

        origin = parts[0].Trim().ToUpper();
        destination = parts[1].Trim().ToUpper();
        return true;
    }

    private static bool ValidateRouteSearchInput(string input, out string origin, out string destination)
    {
        origin = destination = string.Empty;

        var parts = input?.Split('-');
        if (parts == null || parts.Length != 2)
            return false;

        if (!IsValidCity(parts[0]) || !IsValidCity(parts[1]))
            return false;

        origin = parts[0].Trim().ToUpper();
        destination = parts[1].Trim().ToUpper();
        return true;
    }

    private static bool IsValidCity(string input)
    {
        return !string.IsNullOrWhiteSpace(input) && input.All(char.IsLetter);
    }

    private bool RouteExists(string origin, string destination)
    {
        var routes = _repository.GetRoutes();
        return routes.Any(r => r.Origin == origin && r.Destination == destination);
    }

    private List<Route> GetAvailableRoutes()
    {
        var routes = _repository.GetRoutes();

        if (routes.Count == 0)
        {
            Console.WriteLine("\nNão há rotas disponíveis para consulta.");
            return [];
        }

        return routes;
    }

    private static bool ValidateRouteExistence(List<Route> routes, string origin, string destination)
    {
        bool originExists = routes.Any(r => r.Origin == origin);
        bool destinationExists = routes.Any(r => r.Destination == destination);

        if (!originExists && !destinationExists)
        {
            Console.WriteLine($"\nErro: A origem '{origin}' e o destino '{destination}' não existem nas rotas disponíveis.");
            return false;
        }
        if (!originExists)
        {
            Console.WriteLine($"\nErro: A origem '{origin}' não existe nas rotas disponíveis.");
            return false;
        }
        if (!destinationExists)
        {
            Console.WriteLine($"\nErro: O destino '{destination}' não existe nas rotas disponíveis.");
            return false;
        }

        return true;
    }

    private static bool ValidateThreeLetterCode(string origin, string destination)
    {
        var errors = new List<string>();

        if (origin.Length != 3 || !origin.All(char.IsLetter))
            errors.Add($"Origem '{origin}' inválida. Deve conter exatamente 3 letras maiúsculas.");

        if (destination.Length != 3 || !destination.All(char.IsLetter))
            errors.Add($"Destino '{destination}' inválido. Deve conter exatamente 3 letras maiúsculas.");
        
        if (origin.Equals(destination, StringComparison.OrdinalIgnoreCase))
            errors.Add($"Origem e destino são iguais ('{origin}'). Por favor, tente novamente com valores diferentes.");

        if (errors.Count != 0)
        {
            Console.WriteLine("\nErro(s):");
            errors.ForEach(Console.WriteLine);
            return false;
        }

        return true;
    }
}