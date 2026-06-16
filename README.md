# Sistema para Restaurante

Sistema web para gerenciamento de pedidos de restaurante, desenvolvido como prova de conceito com arquitetura DDD em Blazor Server.

---

## Tecnologias

| Camada | Tecnologia |
|---|---|
| Frontend | Blazor Server + Bootstrap 5 |
| Backend | ASP.NET Core 8 |
| Banco de dados | SQL Server (via EF Core 8) |
| Autenticação | Cookie Authentication + BCrypt |
| Testes | xUnit + Moq |
| Infraestrutura | Docker + Docker Compose |

### Arquitetura

O projeto segue DDD com separação em quatro camadas:

```
SistemaRestaurante.Domain          → Entidades, erros de domínio, interfaces de repositório
SistemaRestaurante.Application     → Use cases, DTOs
SistemaRestaurante.Infrastructure  → EF Core, repositórios SQL Server, autenticação
SistemaRestaurante.Presentation    → Blazor Server (páginas, componentes, layout)
SistemaRestaurante.Tests           → Testes unitários de domínio e use cases
```

---

## Funcionalidades

### Obrigatórias

- **Cadastro de pedidos** — selecionar produtos do cardápio, definir quantidade, mesa e nome do cliente
- **Exibição por setor** — Cozinha visualiza apenas pratos; Copa visualiza apenas bebidas; filtragem automática por `SetorPreparoId` do produto
- **Atualização de status** — cada setor avança o status do item (Em Preparo → Pronto → Entregue); retroceder é bloqueado no domínio
- **Autenticação** — login por usuário e senha com cookie HTTP-only; cada usuário pertence a um setor
- **Histórico de pedidos** — listagem dos pedidos com todos os itens entregues

### Complementares

- **Auto-refresh** — painel de pedidos e painel de itens por setor atualizam automaticamente a cada 10 segundos
- **Painel geral de pedidos** — visão consolidada dos pedidos por status (Em Preparo / Prontos / Entregues)
- **Validação de domínio** — Result pattern com lista de erros tipados; sem exceções para fluxo de negócio
- **Mensagens de erro amigáveis** — todas as chamadas aos use cases e repositórios são protegidas com try-catch nas páginas

---

## Pontos Opcionais Cobertos

| Ponto | Status | Observação |
|---|---|---|
| **Docker** | ✅ | `Dockerfile` + `docker-compose.yml` com SQL Server e app web |
| **DDD** | ✅ | Quatro camadas, entidades com factory methods, Result pattern, erros tipados |
| **Testes unitários** | ✅ | 25 testes cobrindo domínio e use cases com mocks (xUnit + Moq) |
| **Validação intuitiva** | ✅ | Erros de domínio com mensagens descritivas; feedback visual em todos os formulários |
| **Testes de integração** | ⚠️ | Testes de use cases com repositórios mockados (sem banco real) |

---

## Como Rodar

### Com Docker (recomendado)

**Pré-requisitos:** Docker Desktop instalado e em execução.

1. Clone o repositório
2. Crie o arquivo `.env` na raiz com a senha do SQL Server:
   ```
   ACCEPT_EULA=Y
   MSSQL_DATABASE_NAME=master
   MSSQL_SA_PASSWORD=SuaSenha@123
   ```
3. Suba os containers:
   ```bash
   docker-compose up --build
   ```
4. Acesse [http://localhost:8080](http://localhost:8080)

> O banco `SistemaRestaurante` é criado e populado automaticamente pela migration na primeira execução.

---

### Desenvolvimento local

**Pré-requisitos:** .NET 8 SDK + SQL Server (local ou Docker).

1. Atualize a connection string em `SistemaRestaurante.Presentation/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost,1433;Database=SistemaRestaurante;User Id=sa;Password=SuaSenha@123;TrustServerCertificate=True;"
     }
   }
   ```
2. Aplique as migrations:
   ```bash
   dotnet ef database update \
     --project SistemaRestaurante.Infrastructure \
     --startup-project SistemaRestaurante.Presentation
   ```
3. Execute a aplicação:
   ```bash
   dotnet run --project SistemaRestaurante.Presentation
   ```

---

### Executar os testes

```bash
dotnet test SistemaRestaurante.Tests/SistemaRestaurante.Tests.csproj
```

---

## Usuários padrão (seed)

| Usuário | Setor | Permissão |
|---|---|---|
| Garçon | Salão | Criar pedidos |
| Copa | Copa | Realizar pedidos (bebidas) |
| Cozinha | Cozinha | Realizar pedidos (pratos) |

> A senha dos usuários de seed é definida pelo hash BCrypt no `EFCoreContext`. Altere conforme necessário para o ambiente.

---

## Códigos de erro

Erros seguem o padrão `{ENTIDADE}-{DOMINIO}-{SEQUENCIA}`:

| Prefixo | Entidade |
|---|---|
| `USUA` | Usuário |
| `PEDI` | Pedido |
| `PROD` | Produto |
| `SETO` | Setor |

Domínio (`DOM`) indica erro de regra de negócio; `APP` indica erro de aplicação.

> Como a aplicação foi desenvolvida em Blazor Server, as interações da interface já ocorrem de forma assíncrona e reativa via SignalR, dispensando o uso explícito de Ajax.
