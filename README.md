# PedidosAPI

API desenvolvida como desafio técnico, utilizando **.NET 8**, **RabbitMQ**, **PostgreSQL**, **Docker** e um **Worker Service** para processamento assíncrono.

O objetivo é demonstrar um fluxo completo de comunicação via fila, com publicação de mensagens pela API e consumo pelo Worker.

---

# 🔍 Índice
- [Objetivo](#-objetivo-do-projeto)  
- [Arquitetura](#-arquitetura-geral)  
- [Como executar](#-como-executar-o-projeto)  
- [Como testar](#-como-testar-o-projeto)  

---

# 📌 Objetivo do Projeto

Implementar uma API capaz de:

1. Criar pedidos.  
2. Listar pedidos existentes.  
3. Publicar mensagens no RabbitMQ ao criar um pedido.  
4. Consumir mensagens em um **Worker**, atualizando o status do pedido para `processado`.  
5. Orquestrar todo o ambiente via Docker.

---

# 🏗 Arquitetura Geral

A solução contém 4 serviços:

- **PedidosAPI** — API REST com endpoints para criação e consulta.  
- **PedidoWorker** — serviço que processa pedidos consumindo mensagens da fila.  
- **RabbitMQ (com painel de administração)**  
- **PostgreSQL**

A API aplica automaticamente as migrations ao subir, garantindo que o banco esteja pronto sem a necessidade de rodar comandos para criação da tabela.

### Estrutura do Projeto

```
/PedidosAPI
   /PedidoApi
      Controllers/
      Services/
   /PedidoWorker
      Worker.cs
   /Shared
      Models/
      Data/
   /docker
   docker-compose.yml
```
---

# 🐳 Como executar o projeto

### ✔ Pré-requisitos
- Docker  
- Docker Compose  
- Portas **5068**, **5432** e **15672** livres

### ✔ Clonar o repositório

```bash
git clone https://github.com/iuri-z/PedidosAPI.git
cd PedidosAPI
```
### ✔ Subir todos os serviços

```bash
docker-compose up --build
```

Aguarde até que todos os containers subam.  

### 🌐 Endereços importantes

| Serviço             | URL                            |
|---------------------|--------------------------------|
| API                 | http://localhost:5068          |
| Swagger             | http://localhost:5068/swagger  |
| RabbitMQ Dashboard  | http://localhost:15672         |
| RabbitMQ User/Pass  | guest / guest                  |
| PostgreSQL          | localhost:5432                 |

# 🧪 Como testar o projeto

### 1️⃣ Criar um novo pedido

POST → `http://localhost:5068/pedidos`

#### Body (JSON):

```json
{
  "nomeCliente": "João da Silva",
  "descricao": "Notebook Dell",
  "valor": 3899.90
}
```

#### Resposta esperada:

```json
{
  "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "nomeCliente": "João da Silva",
  "descricao": "Notebook Dell",
  "valor": 3899.9,
  "status": "pendente"
}
```

---

### 2️⃣ Ver a mensagem na fila

Acesse:

```
http://localhost:15672
```

No painel do RabbitMQ:

- clique em **Queues**
- abra **pedidos_queue**

Você verá a mensagem contendo o GUID publicado pela API.

---

### 3️⃣ Ver o Worker processando pedidos

```bash
docker-compose logs -f worker
```

Saída esperada:

```
Worker escutando fila...
Pedido <GUID> processado.
```

---

### 4️⃣ Consultar pedidos processados

GET → `http://localhost:5068/pedidos`

Resposta esperada:

```json
[
  {
    "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "nomeCliente": "João da Silva",
    "descricao": "Notebook Dell",
    "valor": 3899.9,
    "status": "processado"
  }
]
```


