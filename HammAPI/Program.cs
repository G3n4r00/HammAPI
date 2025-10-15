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



// Adicionar a configura��o do appsettings e verificar se as variaveis de ambiente sobrescrevem (para o docker)
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();

// Adiciona o contexto do banco de dados ao cont�iner de servi�os
builder.Services.AddDbContext<HammAPIDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"),
                      npgsqlOptions => npgsqlOptions.EnableRetryOnFailure())
);

// Adicionar o servi�o para gerar e manipular os hashes de senhas
builder.Services.AddSingleton<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

// --- JWT CONFIG ------------------------------------------------------------
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];
var jwtExpiry = jwtSection["ExpiryMinutes"];


// Adiciona os Servi�os controllers, de relatorio e de cotacao

builder.Services.AddScoped<IAuthService, AuthService>();          
builder.Services.AddScoped<IUserRepository, UserRepository>();     
builder.Services.AddScoped<RelatorioService>();
builder.Services.AddHttpClient<CambioService>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // mant�m nomes como est�o
    });


// Configura��o do Swagger 
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

    // Defini��o de seguran�a Bearer
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

// Configura��o da Autentica��o com o JWT Bearer
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
    // Se n�o houver chave, adiciona autentica��o "dummy" para n�o quebrar startup; 
    // por�m endpoints protegidos ir�o falhar quando chamados
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

// Autentica��o deve vir ANTES de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//Apenas popula o DB depois de realizar a migra��o do EF
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HammAPIDbContext>();
    db.Database.Migrate();

    DataPopulation.PopulateDb(app); 
}


app.Run(); // acessar http://localhost:5000/swagger/index.html quando no docker

