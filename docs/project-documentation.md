# Documentação do Projeto - Cash Flow

## Objetivo
Controlar lançamentos financeiros diários, classificados como débitos ou créditos, e disponibilizar um saldo diário consolidado.

## Requisitos de negócio

| Requisito | Atendimento |
|---|---|
| Serviço de controle de lançamentos | `POST /api/v1/cash-entries` |
| Serviço de consolidado diário | `GET /api/v1/daily-balances/{date}` |

## Requisitos técnicos obrigatórios

| Requisito | Atendimento |
|---|---|
| Desenho da solução | `docs/diagrams/architecture.drawio` |
| C# | Solution .NET 8 em `CashFlowChallenge.sln` |
| Testes | Projeto `tests/CashFlow.Tests` |
| Boas práticas | Clean Architecture, SOLID, Repository, Unit of Work, Outbox |
| README | `README.md` |
| Repositório público | Script `scripts/create-github-repo.sh` |
| Documentações no repositório | Pasta `docs` |

## Modelo de dados

### CashEntry
Registra cada lançamento financeiro.

- `Id`: identificador único.
- `EntryDate`: data do lançamento.
- `Type`: 1 para crédito, 2 para débito.
- `Amount`: valor positivo do lançamento.
- `Description`: descrição.
- `CreatedAtUtc`: data/hora de criação.

### DailyBalance
Armazena o consolidado por dia.

- `BalanceDate`: data consolidada.
- `TotalCredits`: total de créditos.
- `TotalDebits`: total de débitos.
- `Balance`: saldo final.
- `UpdatedAtUtc`: última atualização.

### OutboxMessage
Garante resiliência entre gravação de lançamento e consolidação.

- `Id`: identificador da mensagem.
- `Type`: tipo do evento.
- `Payload`: evento serializado.
- `CreatedAtUtc`: criação.
- `ProcessedAtUtc`: processamento.
- `Attempts`: tentativas.
- `LastError`: último erro.

## Fluxo de criação de lançamento

1. Cliente envia `POST /api/v1/cash-entries`.
2. API chama `CreateCashEntryUseCase`.
3. Domínio valida valor e descrição.
4. Aplicação cria o lançamento e a mensagem outbox.
5. Infraestrutura grava ambos na mesma transação.
6. Worker processa a mensagem posteriormente e atualiza o consolidado.

## Fluxo de consulta do consolidado

1. Cliente envia `GET /api/v1/daily-balances/{date}`.
2. API chama `GetDailyBalanceUseCase`.
3. Repositório lê `DailyBalance` por data.
4. API retorna o saldo consolidado ou `404`.

## Trade-offs

A solução usa uma arquitetura modular em vez de microsserviços completos para reduzir complexidade operacional. O desenho permite evolução futura para microserviços, pois o domínio e a aplicação já estão desacoplados da infraestrutura.

## Pontos de evolução

- Separar o worker em processo independente.
- Usar fila externa para maior throughput.
- Adicionar autenticação e autorização.
- Usar cache para consolidado.
- Adicionar métricas de throughput, latência e falhas de consolidação.
