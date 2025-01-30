# Sistema de Gerenciamento de Rotas

Este projeto é um sistema para gerenciar e calcular a melhor rota entre cidades, garantindo a menor distância ou menor custo. O sistema permite registrar novas rotas, consultar rotas existentes e encontrar o melhor caminho entre dois pontos.

## 📌 Funcionalidades

- **Registrar novas rotas:** Insere novas conexões entre localidades com custo associado.
- **Consultar melhor rota:** Busca o caminho mais barato entre dois pontos cadastrados.
- **Validações:** Garante que os dados inseridos estão corretos e evita registros duplicados.
- **Persistência de dados:** Salva e recupera rotas de um arquivo local.

## 🛠️ Tecnologias Utilizadas

- **C# 8.0**  
- **.NET 8.0**  
- **FluentAssertions** (Testes Unitários)  
- **Moq** (Mocking para testes)  
- **XUnit** (Framework de testes)
- **Arquitetura em Camadas**
    - **Application (Regras de negócio)**
    - **Domain (Entidades e Interfaces)**
    - **Infrastructure (Repositório de Dados)**
    - **Presentation (Interface CLI)**
    - **Tests (Testes unitários)**

## 🚀 Como Executar

1. **Clone o repositório**  
   ```sh
   git clone https://github.com/seuusuario/nome-do-repositorio.git
   cd nome-do-repositorio

2. **Compilar o projeto**
    ```sh
    dotnet build

3. **Compilar o projeto**
    ```sh
    dotnet run --project src/Presentation



## 📖 Exemplo de Uso

### 🛠️ **1. Registrar uma nova rota**
Usuário seleciona a opção de registrar uma nova rota:

```sh
Escolha uma opção:
1. Registrar nova rota
2. Consultar melhor rota
3. Sair

Opção: 1  # Usuário escolheu a opção 1

Rotas Disponíveis:
GRU -> BRC, Custo: $10
BRC -> SCL, Custo: $5
GRU -> CDG, Custo: $75
GRU -> SCL, Custo: $20
GRU -> ORL, Custo: $56
ORL -> CDG, Custo: $5
SCL -> ORL, Custo: $20

Exemplo de entrada: GRU,BRC,10

Digite a rota no formato Origem,Destino,Valor: ORL,BRC,25  # Entrada do usuário

Rota registrada com sucesso! ✅

Rotas Disponíveis (atualizadas):
GRU -> BRC, Custo: $10
BRC -> SCL, Custo: $5
GRU -> CDG, Custo: $75
GRU -> SCL, Custo: $20
GRU -> ORL, Custo: $56
ORL -> CDG, Custo: $5
SCL -> ORL, Custo: $20
ORL -> BRC, Custo: $25
```

### 🛠️ **2. Encontrar a melhor rota entre dois destinos**
Usuário seleciona a opção de consultar melhor rota:
```sh
Escolha uma opção:
1. Registrar nova rota
2. Consultar melhor rota
3. Sair

Opção: 2 # Usuário escolheu a opção 2

Rotas Disponíveis:
GRU -> BRC, Custo: $10
BRC -> SCL, Custo: $5
GRU -> CDG, Custo: $75
GRU -> SCL, Custo: $20
GRU -> ORL, Custo: $56
ORL -> CDG, Custo: $5
SCL -> ORL, Custo: $20
ORL -> BRC, Custo: $25

Digite a rota no formato Origem-Destino: BRC-CDG # Entrada do usuário

Melhor Rota: BRC - SCL - ORL - CDG ao custo de $30 
```


### 🛠️ **3. Sair**
Usuário seleciona a opção de sair:
```sh
Escolha uma opção:
1. Registrar nova rota
2. Consultar melhor rota
3. Sair

Opção: 3 # Usuário escolheu a opção 2

Saindo...
```

## 🏗️ Estrutura do Projeto

```bash
📂lumini-projeto/
 ┣ 📂 src/
┃ ┣ 📂 Application/
┃ ┃ ┣ 📂Enums/
┃ ┃ ┃ ┣ 📂 MenuOption.cs
┃ ┃ ┣ 📂Services/
┃ ┃ ┃ ┣ 📂RouteService.cs
┃ ┣ 📂Domain/
┃ ┃ ┣ 📂 Entities/
┃ ┃ ┃ ┣ 📂 Route.cs
┃ ┃ ┣ 📂Interfaces/
┃ ┃ ┃ ┣ 📂 IRouteRepository.cs
┃ ┣ 📂Infrastructure/
┃ ┃ ┣ 📂 Repositories/
┃ ┃ ┃ ┣ 📂RouteRepository.cs
┃ ┣ 📂Presentation/
┃ ┃ ┣ 📂 Program.cs
┃ ┣ 📂 README.md
 ┣ 📂 tests/
┃ ┣ 📂UnitTest/
┃ ┃ ┣ 📂 Application/
┃ ┃ ┃ ┣ 📂Services/
┃ ┃ ┃ ┃ ┣ 📂 RouteServiceTest.cs
┃ ┃ ┣ 📂Helpers/
┃ ┃ ┃ ┣ 📂ConsoleOutput.cs
```
## 🔍 Observação  

Este projeto foi desenvolvido seguindo **práticas de Clean Code e SOLID**, garantindo um código **modular, reutilizável e fácil de manter**.  

###  **Decisões de Design**
- **Arquitetura em Camadas** → Separação das responsabilidades em `Application`, `Domain`, `Infrastructure`, `Presentation` e `Tests` para facilitar manutenção e testes.
- **Inversão de Dependência** → Uso de **Interfaces (`IRouteRepository`)** para permitir **testes unitários** e facilitar a substituição de implementações.
- **Uso de Repository Pattern** → Isolamento da lógica de persistência (`RouteRepository`), garantindo maior flexibilidade e desacoplamento do serviço (`RouteService`).
- **Testes Unitários** → Implementação de **xUnit + Moq + FluentAssertions** para garantir a qualidade do código e prevenir regressões.
- **Validação de Entrada** → Métodos de validação para garantir que os dados estejam no formato correto antes de serem processados.

Essas decisões foram adotadas para garantir **escalabilidade, flexibilidade e qualidade do código**. 

###  **Persistência**
Atualmente, a persistência das rotas é realizada por meio de **arquivos de texto (`routes.txt`)**, garantindo uma solução simples e funcional. No entanto, a aplicação foi estruturada para que, caso necessário, seja fácil **substituir a persistência por um banco de dados relacional ou NoSQL**, como **SQL Server, PostgreSQL ou MongoDB**, apenas implementando uma nova versão do `IRouteRepository`.

Essas decisões foram adotadas para garantir **escalabilidade, flexibilidade e qualidade do código**. 🚀


## ⚙️ Requisitos
Para executar este projeto, certifique-se de ter os seguintes requisitos instalados:
- 🔧 **.NET 8.0 SDK** instalado
- 🖥️ **Visual Studio 2022** (ou outra IDE compatível)

## 🧑‍💻 Contribuição
Fique à vontade para contribuir!
- Faça um Fork
- Crie uma branch (git checkout -b feature/nova-feature)
- Faça um commit (git commit -m 'Adicionando nova feature')
- Envie um PR


## 👨‍💻 Autor

**Andrei Jung**  
**Email:** jung.andrei@gmail.com  
**LinkedIn:** [LinkedIn](https://www.linkedin.com/in/andrei-jung/)
