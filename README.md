# DESCRIÇÃO DO PROJETO
A Hamm API (nomeada carinhosamente em homenagem ao porquinho-cofrinho do filme Toy Story) é uma aplicação desenvolvida em ASP.NET Core Web API com Entity Framework Core, cujo principal objetivo é atuar como o motor de um sistema de gestão financeira pessoal.
Ela é responsável por centralizar e disponibilizar, de forma segura e organizada, todas as informações relacionadas a:
•	Gastos pessoais
•	Metas financeiras
•	Budgets mensais
•	Relatórios consolidados
A API funciona como o ponto central de comunicação entre um aplicativo cliente (por exemplo, um app mobile) e o banco de dados, garantindo que todas as operações de cadastro, consulta, atualização e exclusão de dados sejam realizadas de forma consistente.
Além disso, a Hamm API implementa a lógica de negócios necessária para:
•	Processar e integrar dados financeiros.
•	Gerar relatórios (JSON).
•	Executar consultas avançadas utilizando LINQ.
•	Integrar informações externas, como taxas de câmbio em tempo real, por meio de APIs públicas.
Com isso, o projeto busca oferecer uma solução modular e extensível, capaz de evoluir futuramente para um sistema completo de gestão financeira pessoal com interface mobile ou web, mantendo a API como núcleo central.


# DESIGN DO SISTEMA
<img width="945" height="563" alt="image" src="https://github.com/user-attachments/assets/4938f870-99a1-454f-871f-5a82d8bb7f26" />


# Diagrama de Entidades (Banco de Dados)
<img width="945" height="633" alt="image" src="https://github.com/user-attachments/assets/39f470db-617c-4056-9acf-1c75385199d5" />


# Como Rodar o Projeto
1️⃣ Pré-requisitos

* Docker
   instalado

* Docker Compose
   instalado

* (Opcional) Visual Studio ou VS Code para edição do código

2️⃣ Clonar o repositório
```bash
git clone <URL_DO_REPOSITORIO>
cd <PASTA_DO_PROJETO>
```

3) Configurar variáveis de ambiente

O projeto já possui as variáveis de conexão no docker-compose.yml:
```bash
environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - ConnectionStrings__Default=Host=db;Port=5432;Database=hammapi;Username=postgres;Password=postgres
```

Você pode alterar Username, Password ou Database se desejar.

3️⃣ Construir e subir os containers
```bash
docker-compose up --build
```

Isso vai criar imagens, subir o container da API e o container do banco de dados PostgreSQL.

A primeira vez pode demorar alguns minutos.

4️⃣ Acessar a API

A API estará disponível em:

http://localhost:5000


O Swagger estará disponível em:

http://localhost:5000/swagger


⚠️ Caso você altere a porta no docker-compose.yml, substitua 5000 pela porta configurada.

5️⃣ Parar os containers
```bash
docker-compose down
```

Isso para e remove os containers sem deletar volumes de dados do banco.

6️⃣ Dicas Extras

Se quiser limpar tudo (containers, volumes e imagens):

```bash
docker-compose down -v --rmi all
```

Para ver os logs da API em tempo real:
```bash
docker-compose logs -f api
```
