namespace Domain.Entities;

public class Route(string origin, string destination, int cost)
{
    public string Origin { get; } = origin;
    public string Destination { get; } = destination;
    public int Cost { get; } = cost;
}
