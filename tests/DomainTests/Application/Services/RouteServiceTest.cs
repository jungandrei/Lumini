using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using UnitTest.Helpers;

namespace UnitTest.Application.Services;

public class RouteServiceTest
{
    private readonly RouteService _service;
    private readonly Mock<IRouteRepository> _repositoryMock;

    public RouteServiceTest()
    {
        _repositoryMock = new Mock<IRouteRepository>();
        _service = new RouteService(_repositoryMock.Object);
    }

    [Fact]
    public void HandleRegisterRoute_Should_NotRegister_When_InputIsInvalid()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetRoutes()).Returns(new List<Route>());

        var simulatedInput = new StringReader("\n");
        Console.SetIn(simulatedInput);

        using var consoleOutput = new ConsoleOutput();

        // Act
        _service.HandleRegisterRoute();

        // Assert
        consoleOutput.GetOutput().Should().Contain("Formato inválido! Entrada não pode ser vazia.");
    }

    [Theory]
    [InlineData("GRU,BRC")]
    [InlineData("GRU,BRC,TEN")]
    [InlineData("GRU,BRC,")]
    [InlineData("GRU;BRC;10")]
    [InlineData("GRU BRC 10")]
    public void HandleRegisterRoute_Should_NotRegister_When_InputFormatIsInvalid(string invalidInput)
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetRoutes()).Returns(new List<Route>());

        var simulatedInput = new StringReader(invalidInput + "\n");
        Console.SetIn(simulatedInput);

        using var consoleOutput = new ConsoleOutput();

        // Act
        _service.HandleRegisterRoute();

        // Assert
        consoleOutput.GetOutput().Should().Contain("Formato inválido! Use o formato Origem,Destino,Valor.");
    }


    [Fact]
    public void HandleRegisterRoute_Should_NotRegister_When_RouteAlreadyExists()
    {
        // Arrange

        _repositoryMock.Setup(r => r.GetRoutes()).Returns(new List<Route> { new Route("GRU", "BRC", 10) });
        
        var simulatedInput = new StringReader("GRU,BRC,10\n");
        Console.SetIn(simulatedInput);

        using var consoleOutput = new ConsoleOutput();

        // Act
        _service.HandleRegisterRoute();

        // Assert
        consoleOutput.GetOutput().Should().Contain("A rota já existe! Não é possível adicionar duplicadas.");
    }
    
    [Fact]
    public void HandleFindBestRoute_Should_ReturnError_When_NoRoutesAvailable()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetRoutes()).Returns(new List<Route>());
                
        var simulatedInput = new StringReader("GRU-CDG\n");
        Console.SetIn(simulatedInput);

        using var consoleOutput = new ConsoleOutput();

        // Act
        _service.HandleFindBestRoute();

        // Assert
        consoleOutput.GetOutput().Should().Contain("Não há rotas disponíveis para consulta.");
    }
   
    [Fact]
    public void HandleFindBestRoute_Should_ReturnError_When_DestinationDoesNotExist()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetRoutes()).Returns(new List<Route> { new Route("GRU", "BRC", 10) });
                
        var simulatedInput = new StringReader("GRU-CDG\n");
        Console.SetIn(simulatedInput);

        using var consoleOutput = new ConsoleOutput();

        // Act
        _service.HandleFindBestRoute();

        // Assert
        consoleOutput.GetOutput().Should().Contain("Erro: O destino 'CDG' não existe nas rotas disponíveis.");
    }
    
    [Fact]
    public void HandleFindBestRoute_Should_ReturnError_When_OriginDoesNotExist()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetRoutes()).Returns(new List<Route> { new Route("SCL", "BRC", 10) });
        
        var simulatedInput = new StringReader("GRU-BRC\n");
        Console.SetIn(simulatedInput);

        using var consoleOutput = new ConsoleOutput();

        // Act
        _service.HandleFindBestRoute();

        // Assert
        consoleOutput.GetOutput().Should().Contain("Erro: A origem 'GRU' não existe nas rotas disponíveis.");
    }
    
    [Fact]
    public void HandleFindBestRoute_Should_ReturnError_When_OriginAndDestinationDoNotExist()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetRoutes()).Returns(new List<Route> { new Route("SCL", "BRC", 10) });
                
        var simulatedInput = new StringReader("GRU-CDG\n");
        Console.SetIn(simulatedInput);

        using var consoleOutput = new ConsoleOutput();

        // Act
        _service.HandleFindBestRoute();

        // Assert
        consoleOutput.GetOutput().Should().Contain("Erro: A origem 'GRU' e o destino 'CDG' não existem nas rotas disponíveis.");
    }
    
    [Fact]
    public void FindBestRoute_Should_Return_CorrectRoute()
    {
        // Arrange
        var routes = new List<Route>
        {
            new Route("GRU", "BRC", 10),
            new Route("BRC", "CDG", 5),
            new Route("GRU", "CDG", 50)
        };

        _repositoryMock.Setup(r => r.GetRoutes()).Returns(routes);

        // Act
        var result = _service.FindBestRoute("GRU", "CDG");

        // Assert
        result.Should().Be("GRU - BRC - CDG ao custo de $15");
    }
    
    [Fact]
    public void FindBestRoute_Should_Return_NoRoute_When_NoPathExists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetRoutes()).Returns(new List<Route>());

        // Act
        var result = _service.FindBestRoute("GRU", "CDG");

        // Assert
        result.Should().Be("Nenhuma rota encontrada.");
    }

    [Theory]
    [InlineData("AAAA", "BRC", "Origem 'AAAA' inválida. Deve conter exatamente 3 letras maiúsculas.")]
    [InlineData("BBBB", "DEF", "Origem 'BBBB' inválida. Deve conter exatamente 3 letras maiúsculas.")]
    public void HandleRegisterRoute_Should_ReturnError_When_OriginIsInvalid(string origin, string destination, string expectedErrorMessage)
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetRoutes()).Returns(new List<Route>());

        using var consoleOutput = new ConsoleOutput();
                
        var input = $"{origin},{destination},10";
        Console.SetIn(new StringReader(input));

        // Act
        _service.HandleRegisterRoute();

        var output = consoleOutput.GetOutput();

        // Assert
        output.Should().Contain(expectedErrorMessage);
    }
    
    [Theory]
    [InlineData("GRU", "AAAA", "Destino 'AAAA' inválido. Deve conter exatamente 3 letras maiúsculas.")]
    [InlineData("ABC", "BBBB", "Destino 'BBBB' inválido. Deve conter exatamente 3 letras maiúsculas.")]
    public void HandleRegisterRoute_Should_ReturnError_When_DestinationIsInvalid(string origin, string destination, string expectedErrorMessage)
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetRoutes()).Returns(new List<Route>());

        using var consoleOutput = new ConsoleOutput();
                
        var input = $"{origin},{destination},10";
        Console.SetIn(new StringReader(input));

        // Act
        _service.HandleRegisterRoute();

        var output = consoleOutput.GetOutput();

        // Assert
        output.Should().Contain(expectedErrorMessage);
    }
    
    [Fact]
    public void HandleRegisterRoute_Should_RegisterRoute_When_InputIsValid()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetRoutes()).Returns(new List<Route>());

        using var consoleOutput = new ConsoleOutput();
        
        var simulatedInput = new StringReader("GRU,BRC,10\n");
        Console.SetIn(simulatedInput);

        // Act
        _service.HandleRegisterRoute();

        var output = consoleOutput.GetOutput();

        // Assert
        output.Should().Contain("Rota registrada com sucesso!");
        output.Should().NotContain("inválida");
    }
     
    [Fact]
    public void HandleRegisterRoute_Should_ReturnError_When_OriginEqualsDestination()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetRoutes()).Returns(new List<Route>());

        using var consoleOutput = new ConsoleOutput();
        
        var input = "GRU,GRU,10";
        Console.SetIn(new StringReader(input));

        // Act
        _service.HandleRegisterRoute();

        var output = consoleOutput.GetOutput();

        // Assert
        output.Should().Contain("Origem e destino são iguais");
    }
        
    [Fact]
    public void HandleRegisterRoute_Should_ReturnError_When_RouteAlreadyExists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetRoutes()).Returns(new List<Route> { new Route("GRU", "BRC", 10) });

        using var consoleOutput = new ConsoleOutput();
        
        var input = "GRU,BRC,10";
        Console.SetIn(new StringReader(input));

        // Act
        _service.HandleRegisterRoute();

        var output = consoleOutput.GetOutput();

        // Assert
        output.Should().Contain("A rota já existe! Não é possível adicionar duplicadas.");
    }
    
    [Fact]
    public void HandleRegisterRoute_Should_Register_When_RouteDoesNotExist()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetRoutes()).Returns(new List<Route>());

        using var consoleOutput = new ConsoleOutput();
        
        var input = "GRU,BRC,10";
        Console.SetIn(new StringReader(input));

        // Act
        _service.HandleRegisterRoute();

        var output = consoleOutput.GetOutput();

        // Assert
        output.Should().Contain("Rota registrada com sucesso!");
        output.Should().NotContain("A rota já existe!");
    }

    [Fact]
    public void HandleFindBestRoute_Should_NotProceed_When_NoRoutesAvailable()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetRoutes()).Returns(new List<Route>());

        using var consoleOutput = new ConsoleOutput();

        // Act
        _service.HandleFindBestRoute();

        var output = consoleOutput.GetOutput();

        // Assert
        output.Should().Contain("Não há rotas disponíveis para consulta.");
    }

    [Fact]
    public void HandleFindBestRoute_Should_DisplayRoutes_When_TheyExist()
    {
        // Arrange
        var expectedRoutes = new List<Route> { new Route("GRU", "BRC", 10) };
        _repositoryMock.Setup(r => r.GetRoutes()).Returns(expectedRoutes);

        using var consoleOutput = new ConsoleOutput();
        
        var simulatedInput = new StringReader("GRU-BRC\n");
        Console.SetIn(simulatedInput);

        // Act
        _service.HandleFindBestRoute();

        var output = consoleOutput.GetOutput();

        // Assert
        output.Should().Contain("Rotas Disponíveis:");
        output.Should().Contain("GRU -> BRC, Custo: $10");
    }
}