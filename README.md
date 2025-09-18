# 💰 Hamm API

### Gerenciador de Finanças Pessoais

<p align="center">
    <img width="720" height="720" alt="HammAPI" src="https://github.com/user-attachments/assets/17856f95-ca34-4763-b619-8e19e1199f54" />
</p>

---

## 📄 Descrição do Projeto

A **Hamm API** é uma aplicação desenvolvida em ASP.NET Core Web API com Entity Framework Core. [cite_start]Seu principal objetivo é atuar como o motor de um sistema de gestão financeira pessoal, centralizando e organizando informações de forma segura.

Ela é responsável por gerenciar:

* **Gastos pessoais** 
* **Metas financeiras** 
* **Orçamentos mensais (Budgets)**
* **Relatórios consolidados** 

[cite_start]A API atua como ponto central de comunicação entre um aplicativo cliente (como um app mobile) e o banco de dados, garantindo que todas as operações de cadastro, consulta, atualização e exclusão sejam consistentes[cite: 12].

Além disso, a Hamm API implementa a lógica de negócios para:

* Processar e integrar dados financeiros.
* Gerar relatórios em formato JSON.
* Executar consultas avançadas usando LINQ.
* Integrar informações externas, como taxas de câmbio em tempo real, via API públicas.

O projeto foi concebido para ser uma solução modular e extensível, com potencial para se tornar um sistema completo de gestão financeira com interfaces mobile ou web, mantendo a API como núcleo central.

---

## 🎨 Design do Sistema

### Diagrama de Componentes

O diagrama a seguir representa a arquitetura de componentes do sistema, mostrando as camadas e suas interações.

![Diagrama de Componentes da Hamm API](https://github.com/user-attachments/assets/4938f870-99a1-454f-871f-5a82d8bb7f26)

---

### Diagrama de Entidades (Banco de Dados)

Este diagrama detalha as entidades e seus relacionamentos no banco de dados.

<p align="center">
    <img width="712" height="477" alt="hammapi - public" src="https://github.com/user-attachments/assets/4d99e054-2651-412f-bf88-479eab7a335f" />
</p>


---

## 🚀 Como Rodar o Projeto

### Pré-requisitos

Certifique-se de que as seguintes ferramentas estão instaladas em seu ambiente:

* **Docker**
* **Docker Compose**
* **(Opcional)** Visual Studio ou VS Code para editar o código

### Passos

1.  **Clonar o repositório**

    Abra o terminal e execute os seguintes comandos:

    ```bash
    git clone <URL_DO_REPOSITORIO>
    cd <PASTA_DO_PROJETO>
    ```

2.  **Configurar variáveis de ambiente**

    O arquivo `docker-compose.yml` já contém as variáveis de conexão. Você pode alterá-las se precisar:

    ```yml
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__Default=Host=db;Port=5432;Database=hammapi;Username=postgres;Password=postgres
    ```

3.  **Construir e iniciar os containers**

    Este comando irá construir as imagens e iniciar os containers da API e do banco de dados PostgreSQL. A primeira execução pode levar alguns minutos.

    ```bash
    docker-compose up --build
    ```

4.  **Acessar a API**

    A API estará disponível em `http://localhost:5000`. O **Swagger UI** para testar os endpoints estará em `http://localhost:5000/swagger`.

    ⚠️ **Atenção**: Se você alterar a porta no `docker-compose.yml`, lembre-se de atualizar o endereço.

5.  **Parar os containers**

    Para parar e remover os containers sem excluir os volumes de dados do banco:

    ```bash
    docker-compose down
    ```

### Dicas Extras

* Para limpar tudo (containers, volumes e imagens), use:
    ```bash
    docker-compose down -v --rmi all
    ```

* Para ver os logs da API em tempo real:
    ```bash
    docker-compose logs -f api
    ```
