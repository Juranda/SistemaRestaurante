# Prova de Conceito - Sistema para Restaurante

Desenvolver um sistema para restaurante que permita gerenciar pedidos de maneira eficiente.

---

### Funcionalidades Principais 

1. Cadastro de Pedidos 
    - O usuário pode selecionar produtos que compõem o cardápio. 
    - Definir a quantidade, mesa e nome do solicitante. 
2. Exibição para Setores 
    - A "Cozinha" deve visualizar apenas os pedidos de pratos. 
    - A "Copa" deve visualizar apenas os pedidos de bebidas. 
3. Atualização de Status 
    - Cada setor pode alterar o status do pedido (Em preparo, Pronto, 
Entregue). 
4. Autenticação de Usuários 
    - O usuário e senha para acessar o sistema. 
5. Histórico de Pedidos 
    - Listagem dos pedidos finalizados. 

---

### Técnologias Utilizadas

##### Frontend
- Bootstrap

##### Backend
- Blazor
- SQL Server


#### Extras
- Testes unitários
- Testes de integração
- Validação intuitiva
- Docker
- DDD

> [!NOTE] Como a aplicação foi desenvolvida utilizando Blazor, as interações da interface já ocorrem de forma assíncrona e reativa, dispensando a necessidade de Ajax explícito.

---

#### Erros


Códigos de erro estão documentados nesta forma:

{ENTIDADE}-{DOMINIO}-{SEQUENCIA}

Entidade:
- USUA - Usuario
- PEDI - Pedido
- PROD - Produto
- MESA - Mesa
- CARD - Cardapio
- SETO - Setor

Dominio: Determina o nivel do erro dentro da aplicação
- DOM - Dominio
- APP - Aplicação
- INF - Infraestrutura
- APR - Apresentacao