using Application.Enums;
using Application.Services;
using Infrastructure.Repositories;

public class Program
{
    public static void Main(string[] args)
    {
        var repository = new RouteRepository();
        var service = new RouteService(repository);

        while (true)
        {
            Console.WriteLine("\nEscolha uma opção:");
            Console.WriteLine("1. Registrar nova rota");
            Console.WriteLine("2. Consultar melhor rota");
            Console.WriteLine("3. Sair");
            Console.Write("\nOpção: ");

            if (!Enum.TryParse(Console.ReadLine(), out MenuOption option))
            {
                Console.WriteLine("\nOpção inválida! Tente novamente.");
                continue;
            }

            switch (option)
            {
                case MenuOption.RegisterRoute:
                    service.HandleRegisterRoute();
                    break;

                case MenuOption.FindBestRoute:
                    service.HandleFindBestRoute();
                    break;

                case MenuOption.Exit:
                    Console.WriteLine("\nSaindo...");
                    return;

                default:
                    Console.WriteLine("\nOpção inválida! Tente novamente.");
                    break;
            }
        }
    }
}