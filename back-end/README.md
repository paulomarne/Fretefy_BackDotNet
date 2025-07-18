# Fretefy Test - API de Gerenciamento de Regiões

Este projeto implementa uma API REST para gerenciamento de regiões e suas respectivas cidades, desenvolvida em ASP.NET Core 3.1 seguindo os princípios do Domain-Driven Design (DDD).

## 📋 Funcionalidades

- **CRUD de Regiões**: Criar, listar, atualizar e remover regiões
- **Gerenciamento de Cidades**: Cada região pode conter múltiplas cidades
- **Ativação/Desativação**: Possibilidade de ativar ou desativar regiões
- **Exportação para Excel**: Exportar listagem de regiões em formato Excel
- **Validações de Negócio**: Validações robustas conforme regras de negócio
- **Documentação Swagger**: Interface interativa para teste da API

## 🏗️ Arquitetura

O projeto segue o padrão DDD (Domain-Driven Design) com a seguinte estrutura:

### Camadas

- **Domain** (`Fretefy.Test.Domain`): Contém as entidades, interfaces e regras de negócio
- **Infrastructure** (`Fretefy.Test.Infra`): Implementações de repositórios e acesso a dados
- **WebApi** (`Fretefy.Test.WebApi`): Controllers, DTOs e configurações da API
- **Tests** (`Fretefy.Test.Tests`): Testes unitários e de integração

### Entidades Principais

#### Regiao
- `Id`: Identificador único (Guid)
- `Nome`: Nome da região (obrigatório, único)
- `Ativo`: Status da região (ativo/inativo)
- `DataCriacao`: Data de criação
- `DataAtualizacao`: Data da última atualização
- `Cidades`: Lista de cidades da região

#### RegiaoCidade
- `Id`: Identificador único (Guid)
- `RegiaoId`: Referência para a região
- `Cidade`: Nome da cidade (obrigatório)
- `UF`: Unidade Federativa (obrigatório, 2 caracteres)

## 🔧 Tecnologias Utilizadas

- **ASP.NET Core 3.1**: Framework web
- **Entity Framework Core 5.0**: ORM para acesso a dados
- **SQLite**: Banco de dados
- **Swagger/OpenAPI**: Documentação da API
- **EPPlus**: Geração de arquivos Excel
- **xUnit**: Framework de testes
- **Moq**: Framework para mocks em testes
- **FluentAssertions**: Assertions fluentes para testes

## 🚀 Como Executar

### Pré-requisitos

- .NET Core 3.1 SDK
- Visual Studio 2019+ ou VS Code

### Passos

1. **Clone o repositório**
   ```bash
   git clone <url-do-repositorio>
   cd Fretefy_BackDotNet/back-end
   ```

2. **Restaurar dependências**
   ```bash
   dotnet restore
   ```

3. **Executar migrações** (se necessário)
   ```bash
   dotnet ef database update --project Fretefy.Test.WebApi
   ```

4. **Executar a aplicação**
   ```bash
   dotnet run --project Fretefy.Test.WebApi
   ```

5. **Acessar a documentação Swagger**
   - Navegue para: `https://localhost:5001` ou `http://localhost:5000`

## 📚 Endpoints da API

### Regiões

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/regiao` | Lista todas as regiões |
| GET | `/api/regiao/{id}` | Obtém uma região específica |
| POST | `/api/regiao` | Cria uma nova região |
| PUT | `/api/regiao/{id}` | Atualiza uma região existente |
| DELETE | `/api/regiao/{id}` | Remove uma região |
| PATCH | `/api/regiao/{id}/toggle-status` | Alterna o status da região |
| GET | `/api/regiao/export` | Exporta regiões para Excel |

### Exemplos de Uso

#### Criar uma nova região

```json
POST /api/regiao
{
  "nome": "Região Sudeste",
  "cidades": [
    {
      "cidade": "São Paulo",
      "uf": "SP"
    },
    {
      "cidade": "Rio de Janeiro",
      "uf": "RJ"
    }
  ]
}
```

#### Atualizar uma região

```json
PUT /api/regiao/{id}
{
  "nome": "Região Sudeste Atualizada",
  "cidades": [
    {
      "id": "existing-city-id",
      "cidade": "São Paulo",
      "uf": "SP"
    },
    {
      "cidade": "Belo Horizonte",
      "uf": "MG"
    }
  ]
}
```

## ✅ Validações Implementadas

### Região
- Nome é obrigatório
- Nome deve ser único
- Deve ter pelo menos uma cidade
- Não pode ter cidades duplicadas (mesma cidade/UF)

### Cidade
- Nome da cidade é obrigatório
- UF é obrigatória e deve ter exatamente 2 caracteres

## 🧪 Testes

O projeto inclui uma suíte completa de testes:

### Executar todos os testes
```bash
dotnet test
```

### Tipos de Testes

- **Testes Unitários**: Testam a lógica de negócio isoladamente
- **Testes de Integração**: Testam a integração entre camadas
- **Testes de Repository**: Testam operações de banco de dados
- **Testes de Controller**: Testam endpoints da API

### Cobertura de Testes

Os testes cobrem:
- Validações de negócio
- Operações CRUD
- Cenários de erro
- Integração com banco de dados
- Endpoints da API

## 📁 Estrutura de Pastas

```
back-end/
├── Fretefy.Test.Domain/
│   ├── Entities/
│   │   ├── Cidade.cs
│   │   ├── IEntity.cs
│   │   ├── Regiao.cs
│   │   └── RegiaoCidade.cs
│   ├── Interfaces/
│   │   ├── Repositories/
│   │   │   ├── ICidadeRepository.cs
│   │   │   └── IRegiaoRepository.cs
│   │   └── Services/
│   │       ├── ICidadeService.cs
│   │       └── IRegiaoService.cs
│   └── Services/
│       ├── CidadeService.cs
│       └── RegiaoService.cs
├── Fretefy.Test.Infra/
│   └── EntityFramework/
│       ├── Mappings/
│       │   ├── CidadeMap.cs
│       │   ├── RegiaoMap.cs
│       │   └── RegiaoCidadeMap.cs
│       ├── Repositories/
│       │   ├── CidadeRepository.cs
│       │   └── RegiaoRepository.cs
│       └── TestDbContext.cs
├── Fretefy.Test.WebApi/
│   ├── Controllers/
│   │   ├── CidadeController.cs
│   │   └── RegiaoController.cs
│   ├── DTOs/
│   │   └── RegiaoDto.cs
│   ├── Data/
│   │   └── Test.db
│   ├── Program.cs
│   └── Startup.cs
└── Fretefy.Test.Tests/
    ├── Domain/
    │   └── RegiaoServiceTests.cs
    ├── Infra/
    │   └── RegiaoRepositoryTests.cs
    ├── WebApi/
    │   └── RegiaoControllerTests.cs
    └── Helpers/
        └── TestDbContextHelper.cs
```

## 🔒 Configurações de Segurança

- CORS configurado para permitir requisições de qualquer origem (desenvolvimento)
- Validação de entrada em todos os endpoints
- Tratamento de exceções centralizado

## 📈 Melhorias Futuras

- Implementação de autenticação e autorização
- Logging estruturado
- Cache de dados
- Paginação para listagens
- Versionamento da API
- Integração com APIs externas para obter coordenadas das cidades
- Implementação de AppService (camada adicional do DDD)

## 🤝 Contribuição

Este projeto foi desenvolvido como parte de um teste técnico para a Fretefy. Para dúvidas ou sugestões, entre em contato através do email: christian.saddock@fretefy.com.br

## 📄 Licença

Este projeto é propriedade da Fretefy e foi desenvolvido para fins de avaliação técnica.

