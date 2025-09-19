using Bogus;
using HammAPI.Data;
using HammAPI.Models;
namespace HammAPI.Utils
{
    public class DataPopulation
    {
        public static void PopulateDb(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HammAPIDbContext>();

                // só roda se o banco estiver vazio
                if (!db.Usuarios.Any())
                {
                    var faker = new Faker("pt_BR");

                    var categorias = new List<Categoria>
                    {
                        new Categoria { Id = Guid.NewGuid(), Nome = "Alimentação", Tipo = "Despesa", EPadrao = true },
                        new Categoria { Id = Guid.NewGuid(), Nome = "Transporte", Tipo = "Despesa", EPadrao = true },
                        new Categoria { Id = Guid.NewGuid(), Nome = "Salário", Tipo = "Receita", EPadrao = true },
                        new Categoria { Id = Guid.NewGuid(), Nome = "Lazer", Tipo = "Despesa", EPadrao = true }
                    };

                    // adicionar mais categorias aleatórias
                    for (int i = 0; i < 6; i++)
                    {
                        categorias.Add(new Categoria
                        {
                            Id = Guid.NewGuid(),
                            Nome = faker.Commerce.Department(),
                            Tipo = faker.PickRandom(new[] { "Receita", "Despesa" }),
                            EPadrao = false,
                            Descricao = faker.Lorem.Sentence()
                        });
                    }

                    
                    var usuarios = new List<Usuario>();
                    for (int i = 0; i < 5; i++)
                    {
                        var usuario = new Usuario
                        {
                            Id = Guid.NewGuid(),
                            PrimeiroNome = faker.Name.FirstName(),
                            UltimoNome = faker.Name.LastName(),
                            Email = faker.Internet.Email(),
                            SenhaHash = "123" 
                        };

                        usuarios.Add(usuario);
                    }

                    // para cada usuario
                    var metas = new List<Meta>();
                    var orcamentos = new List<Orcamento>();
                    var transacoes = new List<Transacao>();

                    foreach (var usuario in usuarios)
                    {
                        // Metas
                        for (int i = 0; i < 2; i++)
                        {
                            metas.Add(new Meta
                            {
                                Id = Guid.NewGuid(),
                                UsuarioId = usuario.Id,
                                Nome = faker.Commerce.ProductName(),
                                ValorObjetivo = faker.Random.Decimal(500, 5000),
                                ValorAtual = faker.Random.Decimal(0, 1000),
                                DataInicio = DateTime.SpecifyKind(DateTime.Now.AddMonths(-faker.Random.Int(1, 6)), DateTimeKind.Utc),
                                DataAlvo = DateTime.SpecifyKind(DateTime.Now.AddMonths(faker.Random.Int(3, 12)), DateTimeKind.Utc),
                                Descricao = faker.Lorem.Sentence(),
                                Status = faker.PickRandom(new[] { "EmProgresso", "Concluida", "Cancelada" })
                            });
                        }

                        // Orçamentos
                        for (int i = 0; i < 2; i++)
                        {
                            var mes = faker.Date.Recent().Month.ToString();
                            var ano = DateTime.Now.Year.ToString();

                            orcamentos.Add(new Orcamento
                            {
                                Id = Guid.NewGuid(),
                                UsuarioId = usuario.Id,
                                Nome = $"Orçamento {mes}/{ano}",
                                ValorLimite = faker.Random.Decimal(500, 3000),
                                ValorUtilizado = faker.Random.Decimal(100, 2000),
                                Mes = mes,
                                Ano = ano
                            });
                        }

                        // Transações
                        for (int i = 0; i < 10; i++)
                        {
                            var categoria = faker.PickRandom(categorias);

                            transacoes.Add(new Transacao
                            {
                                Id = Guid.NewGuid(),
                                UsuarioId = usuario.Id,
                                CategoriaId = categoria.Id,
                                Valor = faker.Random.Decimal(10, 500),
                                Data = DateTime.SpecifyKind(faker.Date.Recent(90), DateTimeKind.Utc),
                                Descricao = faker.Commerce.ProductName(),
                                Tipo = categoria.Tipo,
                                MetodoPagamento = faker.PickRandom(new[] { "CREDITO", "DEBITO" })
                            });
                        }
                    }

                    // Salvar no banco
                    db.Categorias.AddRange(categorias);
                    db.Usuarios.AddRange(usuarios);
                    db.Metas.AddRange(metas);
                    db.Orcamentos.AddRange(orcamentos);
                    db.Transacoes.AddRange(transacoes);

                    db.SaveChanges();
                }
            }
        }

    }
}
