using Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Configuration;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Data.SqlClient;
using MediatR;
using Api.Behaviours;
using Microsoft.AspNetCore.Routing;

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

            // CORS - Permitir requisições de qualquer site
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
                
            // Registrar explicitamente os controladores a partir de Assembly
            var controllerAssembly = typeof(Program).Assembly;
            var controllerTypes = controllerAssembly.GetTypes()
                .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract)
                .ToList();
                
            Console.WriteLine($"Encontrados {controllerTypes.Count} controllers:");
            foreach (var type in controllerTypes)
            {
                Console.WriteLine($"- {type.Name}");
            }

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
                else
                {
                    // Log de aviso se o arquivo XML não for encontrado
                    Console.WriteLine($"AVISO: O arquivo de documentação XML não foi encontrado em {xmlPath}");
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
                
                // Evitar erros de referência circular na serialização
                options.CustomSchemaIds(type => type.FullName);
            });

            var app = builder.Build();

            // Tratamento de exceções global
            app.UseExceptionHandler("/error");
            
            // Configuração de desenvolvimento
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                
                var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                app.UseSwaggerUI(options =>
                {
                    // Configurar endpoints para cada versão disponível
                    if (apiVersionDescriptionProvider.ApiVersionDescriptions.Any())
                    {
                        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                        {
                            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                                $"Scraper de Redes Sociais {description.GroupName.ToUpperInvariant()}");
                        }
                    }
                    else
                    {
                        // Fallback se não houver descrições de versão
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Scraper de Redes Sociais V1");
                    }
                    
                    options.RoutePrefix = "swagger";
                    options.DocumentTitle = "Documentação API - Scraper de Redes Sociais";
                    options.DefaultModelsExpandDepth(-1);
                    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                    options.EnableDeepLinking();
                    options.DisplayRequestDuration();
                });
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            // IMPORTANTE: A ordem correta do pipeline é crítica
            app.UseRouting();
            app.UseCors("AllowAll");
            
            app.UseAuthentication();
            app.UseAuthorization();
            
            // Mapear controllers para endpoints
            app.MapControllers();

            // Registrar os endpoints nos logs
            Console.WriteLine("========== CONFIGURAÇÃO DA API ==========");
            Console.WriteLine("API iniciada com sucesso!");
            Console.WriteLine("Swagger disponível em: http://localhost:5149/swagger");
            Console.WriteLine("=======================================");

            app.Run();
        }
    }
}