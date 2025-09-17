using HammAPI.Data;
using HammAPI.Models;
using HammAPI.Services;
using HammAPI.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;


var builder = WebApplication.CreateBuilder(args);

// Adicionar a configuração do appsettings e verificar se as variaveis de ambiente sobrescrevem (para o docker)
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();

// Adiciona o contexto do banco de dados ao contêiner de serviços
builder.Services.AddDbContext<HammAPIDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"),
                      npgsqlOptions => npgsqlOptions.EnableRetryOnFailure())
);

// Adicionar o serviço para gerar e manipular os hashes de senhas
builder.Services.AddSingleton<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();


// Adiciona os Serviços controllers, de relatorio e de cotacao
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<RelatorioService>();
builder.Services.AddHttpClient<CambioService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//Apenas popula o DB depois de realizar a migração do EF
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HammAPIDbContext>();
    db.Database.Migrate();

    DataPopulation.PopulateDb(app); 
}

// Abrindo no 0.0.0.0 porque faz o app acessível fora do container
app.Run("http://0.0.0.0:80");
