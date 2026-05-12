# Cash Flow Architecture Challenge

Solução proposta para o desafio de Arquiteto de Software: controle de lançamentos de fluxo de caixa com consolidado diário resiliente.

## Visão geral

O sistema possui dois fluxos principais:

1. **Controle de lançamentos**: recebe débitos e créditos via API REST.
2. **Consolidado diário**: consolida o saldo por data de forma assíncrona.

A principal decisão arquitetural foi aplicar o padrão **Transactional Outbox** para que o serviço de lançamento não fique indisponível caso o processamento do consolidado falhe. O lançamento é gravado no SQL Server junto com uma mensagem pendente na tabela `OutboxMessage`. Um worker interno processa essas mensagens e atualiza a tabela `DailyBalance`.

## Arquitetura

Projetos da solution:

```text
src/
  CashFlow.Api             # Controllers, Swagger, HealthCheck e bootstrap HTTP
  CashFlow.Application     # Casos de uso, DTOs e contratos/ports
  CashFlow.Domain          # Entidades, regras de negócio e exceções
  CashFlow.Infrastructure  # EF Core, repositórios e worker de consolidação
tests/
  CashFlow.Tests           # Testes unitários com xUnit, Moq e FluentAssertions
docs/
  diagrams/architecture.drawio
  adr/0001-architecture-decision.md
scripts/
  sql/001-create-database.sql
  sql/002-seed-sample-data.sql
```

## Decisões técnicas

- **C# / .NET 8** para implementação.
- **SQL Server** como banco relacional já existente, sem migrations no fluxo principal.
- **Clean Architecture simplificada** com separação entre API, Application, Domain e Infrastructure.
- **SOLID** aplicado por meio de contratos, injeção de dependência e separação de responsabilidades.
- **Transactional Outbox** para desacoplar gravação de lançamento e consolidação diária.
- **HealthCheck** para monitoramento operacional.
- **Swagger** para documentação e teste manual dos endpoints.
- **xUnit + Moq + FluentAssertions + Coverlet** para testes unitários e cobertura.

## Banco de dados

Execute o script:

```bash
scripts/sql/001-create-database.sql
```

Opcionalmente, execute:

```bash
scripts/sql/002-seed-sample-data.sql
```

Tabelas principais:

- `CashEntry`: lançamentos de débito e crédito.
- `DailyBalance`: saldo consolidado por dia.
- `OutboxMessage`: mensagens pendentes para processamento assíncrono.

## Como rodar localmente

### 1. Subir SQL Server com Docker

```bash
docker compose up -d
```

### 2. Criar banco e tabelas

Conecte no SQL Server local e execute:

```sql
:r scripts/sql/001-create-database.sql
```

Ou abra o arquivo no SQL Server Management Studio / Azure Data Studio e execute manualmente.

### 3. Restaurar pacotes e compilar

```bash
dotnet restore CashFlowChallenge.sln
dotnet build CashFlowChallenge.sln
```

### 4. Rodar a API

```bash
dotnet run --project src/CashFlow.Api/CashFlow.Api.csproj
```

Acesse:

```text
https://localhost:5001/swagger
```

## Endpoints principais

### Criar lançamento

```http
POST /api/v1/cash-entries
Content-Type: application/json
```

Exemplo:

```json
{
  "entryDate": "2025-06-01",
  "type": 1,
  "amount": 100.50,
  "description": "Venda no balcão"
}
```

`type`:

- `1` = Crédito
- `2` = Débito

### Consultar consolidado diário

```http
GET /api/v1/daily-balances/2025-06-01
```

## Testes e cobertura

Execute:

```bash
dotnet test CashFlowChallenge.sln --collect:"XPlat Code Coverage"
```

Os testes cobrem:

- Regras de domínio de lançamento.
- Cálculo do consolidado diário.
- Caso de uso de criação de lançamento.
- Caso de uso de consulta do consolidado.

Meta esperada: cobertura mínima de 80% nas camadas Domain e Application.

## Requisito não funcional atendido

> O serviço de controle de lançamento não deve ficar indisponível se o sistema de consolidado diário cair.

Atendimento:

- A criação do lançamento não depende diretamente da consolidação.
- O lançamento e o evento de consolidação são gravados na mesma transação.
- Se o worker falhar, a mensagem continua pendente na `OutboxMessage`.
- O processamento pode ser retomado posteriormente sem perda de dados.

## Escalabilidade e resiliência

Para 50 requisições por segundo no consolidado diário:

- Índice em `DailyBalance.BalanceDate` pela chave primária.
- Leitura direta por data.
- Possibilidade de cache Redis para reduzir carga no banco.
- Worker processando lotes de mensagens do outbox.
- Possibilidade futura de mover o worker para serviço separado e utilizar fila dedicada.

## Segurança

Implementações e recomendações:

- HTTPS habilitado.
- Validação de entrada no domínio.
- SQL parametrizado via EF Core.
- HealthCheck sem exposição de dados sensíveis.
- Evolução recomendada: JWT/OAuth2, rate limiting, API Gateway e logs auditáveis.

## Observabilidade

Já preparado:

- Logs via `ILogger`.
- Endpoint `/health`.

Evoluções recomendadas:

- OpenTelemetry para traces distribuídos.
- Prometheus/Grafana para métricas.
- Alertas para mensagens com muitas tentativas no outbox.

## Como publicar no GitHub

Pré-requisitos:

- Git instalado.
- GitHub CLI (`gh`) autenticado.

Execute na raiz do projeto:

```bash
chmod +x scripts/create-github-repo.sh
./scripts/create-github-repo.sh
```

Antes, altere dentro do script:

```bash
GITHUB_USER="SEU_USUARIO_GITHUB"
```

## Evoluções futuras

- Separar o consolidado em um microserviço independente.
- Substituir outbox interno por RabbitMQ, Kafka, Azure Service Bus ou AWS SQS.
- Implementar autenticação JWT.
- Adicionar idempotência por `Idempotency-Key` no endpoint de lançamento.
- Adicionar cache Redis para leitura do consolidado.
- Implementar testes de integração com Testcontainers.
- Adicionar dashboards de observabilidade.
