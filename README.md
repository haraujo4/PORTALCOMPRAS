# Avaliação Desenvolvedor .NET

## Descrição do Projeto

Este projeto é uma aplicação web desenvolvida como parte de uma avaliação para a posição de desenvolvedor .NET. A aplicação segue a arquitetura Domain Driven Design (DDD), usa .NET 6.0 e implementa uma API Web com OpenAPI. A persistência de dados é feita usando Entity Framework Core com suporte a SQLite e outro banco de dados relacional.

## Objetivo

O objetivo do projeto é demonstrar habilidades no desenvolvimento de aplicações .NET usando boas práticas de arquitetura, testes unitários e design de APIs.

## Instruções de Construção

1. Clone o repositório:
    ```bash
    git clone https://github.com/seu-usuario/avaliacao-desenvolvedor.git
    ```

2. Navegue até o diretório do projeto:
    ```bash
    cd avaliacao-desenvolvedor
    ```

3. Restaure as dependências:
    ```bash
    dotnet restore
    ```

4. Adicione e aplique migrações:
    ```bash
    Add-Migration _V##versão
    Update-DataBase
    ```

5. Execute o projeto:
    ```bash
    dotnet run WebAPI
    ```

## Instruções de Publicação

Para publicar a aplicação, você pode seguir a documentação oficial do .NET para implantar a aplicação em um servidor ou serviço de hospedagem de sua escolha.

---

Ao seguir esses passos, você terá uma aplicação bem estruturada, com todas as funcionalidades e requisitos necessários para a avaliação.
