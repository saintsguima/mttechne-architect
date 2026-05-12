# ADR 0001 - Arquitetura da solução

## Contexto
O desafio solicita um sistema para controle de lançamentos de fluxo de caixa e um consolidado diário. O requisito não funcional mais relevante é que o serviço de lançamentos não pode ficar indisponível caso o consolidado diário falhe.

## Decisão
Foi escolhida uma arquitetura modular em camadas com separação por projetos:

- `CashFlow.Api`: entrada HTTP REST.
- `CashFlow.Application`: casos de uso, DTOs e contratos.
- `CashFlow.Domain`: entidades, regras de domínio e exceções.
- `CashFlow.Infrastructure`: persistência, repositórios e worker de consolidação.

Para desacoplamento entre lançamento e consolidação, foi adotado o padrão **Transactional Outbox**. O lançamento e a mensagem de integração são gravados na mesma transação. Um worker processa mensagens pendentes e atualiza o consolidado diário de forma assíncrona.

## Consequências
- O serviço de lançamentos continua disponível mesmo se a consolidação estiver indisponível.
- O consolidado passa a ser eventualmente consistente.
- Em evolução futura, o worker pode ser substituído por RabbitMQ, Azure Service Bus, Kafka ou SQS sem mudar o domínio.
