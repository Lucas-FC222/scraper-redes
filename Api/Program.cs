using Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Configuration;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Data.SqlClient;
using MediatR;
using Api.Behaviours;

namespace Api
{
    /// <summary>
    /// Classe principal responsável pela configuração e inicialização da aplicação ASP.NET Core.
    /// Realiza o registro de serviços, middlewares, autenticação, versionamento, Swagger e inicialização do host.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Ponto de entrada da aplicação. Configura todos os serviços, middlewares e executa o host web.
        /// </summary>
        /// <param name="args">Argumentos de linha de comando.</param>
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var serviceAssembly = AppDomain.CurrentDomain.Load("Services");

            builder.Services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = ctx =>
                {
                    ctx.ProblemDetails.Extensions.Add("traceId", ctx.HttpContext.TraceIdentifier);
                    ctx.ProblemDetails.Extensions.Add("instance", $"{ctx.HttpContext.Request.Method}{ctx.HttpContext.Request.Path}");
                };
            });

            builder.Services.AddMediatR(cfg =>
            {
                // Registra os handlers do MediatR
                cfg.RegisterServicesFromAssembly(serviceAssembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            });

            // Configurações do Apify
            builder.Services.Configure<Services.Features.Facebook.Models.ApifySettings>(builder.Configuration.GetSection("Apify"));
            builder.Services.Configure<Services.Features.Instagram.Models.ApifySettings>(builder.Configuration.GetSection("Apify"));

            // Conexão com banco via Dapper
            builder.Services.AddTransient(_ => new SqlConnection(
                builder.Configuration.GetConnectionString("DefaultConnection")
            ));

            // HttpClient para API do Apify
            builder.Services.AddHttpClient();

            builder.Services.ConfigureModules([serviceAssembly], builder.Configuration);

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
                    // Simplified loop using LINQ Select
                    var swaggerEndpoints = apiVersionDescriptionProvider.ApiVersionDescriptions
                        .Select(description => new
                        {
                            Url = $"/swagger/{description.GroupName}/swagger.json",
                            Name = $"Scraper de Redes Sociais {description.GroupName.ToUpperInvariant()}"
                        });

                    foreach (var endpoint in swaggerEndpoints)
                    {
                        options.SwaggerEndpoint(endpoint.Url, endpoint.Name);
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
        }
    }
}