# 🛒 Product Catalog & Search Service

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat&logo=dotnet)
![Architecture](https://img.shields.io/badge/Architecture-Modular%20Monolith-blue)
![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?logo=docker)
![Elasticsearch](https://img.shields.io/badge/Search-Elasticsearch%20v8-005571?logo=elasticsearch)
![Kafka](https://img.shields.io/badge/Event%20Bus-Kafka-231F20?logo=apachekafka)

A reference implementation of a **Modern .NET 10 Modular Monolith** designed for high-scale e-commerce scenarios. This project demonstrates **CQRS (Command Query Responsibility Segregation)** by separating transactional writes (PostgreSQL) from high-performance reads (Elasticsearch), synchronized in near real-time via **Apache Kafka**.

## 🚀 Key Features

* **Modular Monolith Architecture:** Enforced separation of concerns between `Host`, `Modules` (Catalog), and `Shared` infrastructure.
* **CQRS Pattern:**
    * **Write Side:** EF Core 9+ with PostgreSQL for ACID transactions.
    * **Read Side:** Elasticsearch v8 for complex filtering, fuzzy search, and aggregation.
* **Event-Driven Synchronization:** Uses **MassTransit** and **Kafka** to decouple the Write and Read sides. Updates in the DB are pushed to Elastic asynchronously.
* **High Performance:**
    * **Redis Caching:** Distributed caching for frequently accessed entities.
    * **Vertical Slice Architecture:** Features are self-contained (Command + Handler + Endpoint).
* **Modern Tech Stack:** Built on the bleeding edge .NET 10 stable.

## 🏗️ Architecture

The application is split into two distinct paths to optimize for throughput and searchability.

```mermaid
graph TD
    User[Client / Frontend]
    
    subgraph "API Host (.NET 10)"
        API[Web API]
    end

    subgraph "Transactional Zone (Write)"
        Postgres[(PostgreSQL)]
    end

    subgraph "Event Bus"
        Kafka{Apache Kafka}
    end

    subgraph "Search & Read Zone"
        Elastic[(Elasticsearch)]
        Redis[(Redis Cache)]
    end

    %% Flows
    User -->|POST /products| API
    API -->|Save Entity| Postgres
    API -->|Publish ProductSavedEvent| Kafka
    
    Kafka -->|Consume Event| API
    API -->|Index Document| Elastic
    
    User -->|GET /search| API
    API -->|Query| Elastic
    API -->|Get By ID| Redis