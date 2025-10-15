using HammAPI.Data;
using HammAPI.Models;
using HammAPI.Repository;
using HammAPI.Services;
using HammAPI.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

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

// --- JWT CONFIG ------------------------------------------------------------
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];
var jwtExpiry = jwtSection["ExpiryMinutes"];


// Adiciona os Serviços controllers, de relatorio e de cotacao

builder.Services.AddScoped<IAuthService, AuthService>();          
builder.Services.AddScoped<IUserRepository, UserRepository>();     
builder.Services.AddScoped<RelatorioService>();
builder.Services.AddHttpClient<CambioService>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // mantém nomes como estão
    });


// Configuração do Swagger 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Caminho do arquivo XML gerado pelo projeto (se existir)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }

    // Definição de segurança Bearer
    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira 'Bearer {seu token}' para autenticar."
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

// Configuração da Autenticação com o JWT Bearer
if (!string.IsNullOrWhiteSpace(jwtKey))
{
    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // permitir http em desenvolvimento para facilitar testes locais (docker)
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = !string.IsNullOrWhiteSpace(jwtIssuer),
            ValidIssuer = jwtIssuer,
            ValidateAudience = !string.IsNullOrWhiteSpace(jwtAudience),
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
}
else
{
    // Se não houver chave, adiciona autenticação "dummy" para não quebrar startup; 
    // porém endpoints protegidos irão falhar quando chamados
    builder.Services.AddAuthentication();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Autenticação deve vir ANTES de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//Apenas popula o DB depois de realizar a migração do EF
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HammAPIDbContext>();
    db.Database.Migrate();

    DataPopulation.PopulateDb(app); 
}


app.Run(); // acessar http://localhost:5000/swagger/index.html quando no docker

