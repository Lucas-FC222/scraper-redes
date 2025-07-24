using Services;
using Microsoft.Data.SqlClient;
using Api.Middleware;
using Core.Repositories;
using Data;
using Core.Services;
using Infra.Externals.ApiFy.Models;
using Infra.Externals.ApiFy.Interfaces;
using Infra.Externals.ApiFy.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Configuration;
using Infra.Data;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Configurações do Apify
builder.Services.Configure<ApifySettings>(builder.Configuration.GetSection("Apify"));

// Conexão com banco via Dapper
builder.Services.AddTransient<SqlConnection>(_ => new SqlConnection(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

// HttpClient para API do Apify
builder.Services.AddHttpClient();

// Repositórios
builder.Services.AddScoped<IInstagramRepository, InstagramRepository>();
builder.Services.AddScoped<IFacebookRepository, FacebookRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Serviços
builder.Services.AddScoped<IApiFyService, ApiFyService>();
builder.Services.AddScoped<IInstagramService, InstagramService>();
builder.Services.AddScoped<IFacebookService, FacebookService>();
builder.Services.AddScoped<IPostClassifierService, PostClassifierService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Workers
//builder.Services.AddHostedService<FacebookWorker>();
//builder.Services.AddHostedService<InstagramWorker>();
//builder.Services.AddHostedService<NotificationWorker>();

// CORS - Permitir requisições do frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configuração do JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? 
                throw new InvalidOperationException("JWT Key not configured"))
        ),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

// Controllers e Swagger com versionamento
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ignorar valores nulos na serialização JSON
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        
        // Usar camelCase para propriedades
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Adicionar versionamento de API
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // Adiciona cabeçalhos com versões suportadas
});

// Configuração para o Swagger suportar múltiplas versões
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Formato: 'v'major[.minor][-status]
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
// Configurar o Swagger com suporte a versionamento
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    // Configuração para incluir os comentários XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
    
    // Configurar para incluir o versionamento nos endpoints do Swagger
    options.OperationFilter<SwaggerDefaultValues>();

    // Adicionar opção de autenticação JWT ao Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header usando o esquema Bearer.
                      Digite 'Bearer' [espaço] e então seu token.
                      Exemplo: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Registrar o middleware de tratamento global de exceções antes de qualquer outro middleware
app.UseGlobalExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    
    // Obter provedor de descrições de versão
    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    
    app.UseSwaggerUI(options =>
    {
        // Configurar endpoints para cada versão disponível
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json", 
                $"Scraper de Redes Sociais {description.GroupName.ToUpperInvariant()}"
            );
        }
        
        options.RoutePrefix = "swagger"; // Mantém a rota padrão /swagger
        options.DocumentTitle = "Documentação API - Scraper de Redes Sociais";
        options.DefaultModelsExpandDepth(-1); // Oculta a seção de modelos por padrão
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List); // Expande as operações por padrão
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Servir arquivos estáticos do wwwroot

// Usar CORS
app.UseCors("AllowAll");

// Adicionar middleware de autenticação antes de autorização
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
