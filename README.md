# 📦 API de Catálogo de Produtos (.NET 9)

Este projeto é uma API REST desenvolvida como desafio técnico para gerenciamento de produtos, focada em **Clean Architecture** e **Alta Disponibilidade**.

- ⚙️ **Backend**: .NET 9 (Web API)
- 🗄️ **ORM**: Entity Framework Core
- 🐘 **Banco de Dados**: PostgreSQL 15
- 🪣 **Storage**: MinIO (Simulador S3)
- 🐳 **Containerização**: Docker & Docker Compose
- 🧪 **Testes**: xUnit + Moq

---

## 🚀 Como rodar localmente com Docker

### 1. Pré-requisitos

- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)

---

### 2. Clone o projeto

```bash
git clone -b main https://github.com/gutsgon/prova-pratica.git
cd prova-pratica
```

---

### 3. Build e deploy da infraestrutura

---
## ⚠️ Observações

- **Antes de rodar:** Certifique-se de que as portas `5000` (API), `5433` (Postgres) e `9000/9001` (MinIO) estejam livres na sua máquina.
- **Variáveis de Ambiente:** As credenciais de banco e storage já estão pré-configuradas no arquivo `docker-compose.yml` para facilitar a execução local (User/Pass: `admin`/`admin` e `minioadmin`/`minioadmin`).
- **Persistência:** As imagens enviadas são salvas no volume do Docker `minio_data`, persistindo mesmo se reiniciar o container.

---

Execute o comando abaixo para subir todo o ambiente:

```bash
docker compose up -d --build
```

Esse comando irá:

- Subir o **PostgreSQL** (porta `5433`)
- Subir o **MinIO** e criar o bucket `products-images` automaticamente (porta `9000`)
- Buildar e subir a **API .NET** (porta `5000`)

> **Nota:** Na primeira execução, aguarde cerca de 15 segundos para que o banco de dados inicialize completamente.

---

### 4. Configurar o Banco de Dados

Após os containers subirem, execute a migração para criar as tabelas:

```bash
dotnet ef database update -p CatalogoDeProdutos.Infrastructure -s CatalogoDeProdutos.API
```

---

### 5. Acessar a aplicação

- **Swagger (API Docs)**: http://localhost:5000/swagger
- **MinIO Console (Arquivos)**: http://localhost:9001

---

## 🔐 Credenciais de Acesso

A API é aberta, mas para acessar o gerenciador de arquivos (MinIO) ou o banco, use:

- **MinIO Console**: 
  - User: `minioadmin`
  - Pass: `minioadmin`
  
- **PostgreSQL**:
  - User: `admin`
  - Pass: `admin`
  - Database: `catalogodb`

---

## 📦 Funcionalidades de Upload

- Ao cadastrar um produto (`POST /api/Products`), envie a imagem via `multipart/form-data`.
- O arquivo é salvo no MinIO e uma URL pública (`http://localhost:9000/...`) é gerada.
- **Limpeza Automática**: Ao excluir ou atualizar um produto, a imagem antiga é removida automaticamente do storage para economizar espaço.

Você pode acompanhar os logs de upload em tempo real:

```bash
docker compose logs -f api
```

---

## 🧪 Rodar testes automatizados

Para facilitar a correção, um container dedicado para testes. Ele é executado ao usar o comando de build acima `docker compose up -d --build`, os logs dos testes vão executar em segundo plano. Você pode rodar a bateria completa sem instalar o .NET SDK na sua máquina:

```bash
docker compose up tests
```

O resultado dos testes (Passou/Falhou) aparecerá diretamente no seu terminal.

---

## 📂 Estrutura do projeto

O projeto segue estritamente a **Clean Architecture**:

```
/CatalogoDeProdutos
  ├── /CatalogoDeProdutos.API             # Entry Point (Controllers & DI)
  ├── /CatalogoDeProdutos.Application     # Services, DTOs, Interfaces & AutoMapper
  ├── /CatalogoDeProdutos.Domain          # Entidades, Enums & Interfaces de Repositório
  ├── /CatalogoDeProdutos.Infrastructure  # EF Core, PostgreSQL & S3 Implementation
  ├── /CatalogoDeProdutos.Tests           # Testes Unitários (xUnit)
  ├── docker-compose.yml
  └── README.md
```

