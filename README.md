# PedidosAPI

API desenvolvida como desafio técnico, utilizando **.NET**, **RabbitMQ**, **PostgreSQL** e **Docker**.  
O objetivo é demonstrar o fluxo completo de criação e processamento assíncrono de pedidos utilizando filas.

---

## 📌 Objetivo do Projeto

Implementar uma API capaz de:

1. Criar pedidos.
2. Listar pedidos existentes.
3. Publicar mensagens em uma fila RabbitMQ quando um novo pedido é criado.
4. Processar pedidos em um **Worker**, que consome a fila e altera o status de cada pedido para `processado` após a leitura.

---

## 🏗 Arquitetura Geral

A solução é composta pelos seguintes serviços:

- **PedidosAPI**  
  API responsável pelos endpoints e pela publicação das mensagens no RabbitMQ.

- **PedidoWorker**  
  Worker Service que consome mensagens, simula o processamento e atualiza pedidos no banco.

- **PostgreSQL**  
  Banco de dados utilizado pela API e pelo Worker.

- **RabbitMQ (com painel de administração)**  
  Sistema de mensageria usado para comunicação assíncrona.

Toda a estrutura é orquestrada via Docker Compose.

---

## 🐳 Como executar o projeto

### Pré-requisitos
- Docker  
- Docker Compose  

### Subir todos os serviços

```bash
docker-compose up --build
