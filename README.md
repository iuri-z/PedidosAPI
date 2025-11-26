# 📘 PedidosAPI

![.NET Build](https://github.com/iuri-z/PedidosAPI/actions/workflows/dotnet.yml/badge.svg)

API desenvolvida como desafio técnico, utilizando **.NET 8**, **RabbitMQ**, **PostgreSQL**, **Docker** e um **Worker Service** para processamento assíncrono.

O objetivo é demonstrar um fluxo completo de comunicação via fila, com publicação de mensagens pela API e consumo pelo Worker.

---

# 🔍 Índice
- [Objetivo](#-objetivo-do-projeto)  
- [Arquitetura](#-arquitetura-geral)  
- [Como executar](#-como-executar-o-projeto)  
- [Como testar](#-como-testar-o-projeto)  
- [Endpoints](#-endpoints)  
- [Fluxo completo](#-fluxo-completo-do-pedido)  
- [Estrutura do projeto](#-estrutura-do-projeto)  
- [Tecnologias](#-tecnologias-utilizadas)  

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

A API aplica automaticamente as migrations ao subir, garantindo que o banco esteja pronto sem comandos adicionais.

---

# 🐳 Como executar o projeto

## ✔ Pré-requisitos
- Docker  
- Docker Compose  
- Portas **5068**, **5432** e **15672** livres

## ✔ Clonar o repositório

```bash
git clone https://github.com/iuri-z/PedidosAPI.git
cd PedidosAPI
