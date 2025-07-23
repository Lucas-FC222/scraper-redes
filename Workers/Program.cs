using Workers;
using Core;
using Infra;
using Infra.Data;
using Services;
using Microsoft.Data.SqlClient;

var builder = Host.CreateApplicationBuilder(args);

// Carrega configurações do appsettings.json
var configuration = builder.Configuration;

// Configurações do Apify
builder.Services.Configure<ApifySettings>(configuration.GetSection("Apify"));

// Conexão com banco via Dapper
builder.Services.AddTransient<SqlConnection>(_ => new SqlConnection(
    configuration.GetConnectionString("DefaultConnection")
));

// HttpClient para API do Apify
builder.Services.AddHttpClient();

// Repositórios
builder.Services.AddScoped<IInstagramRepository, InstagramRepository>();
builder.Services.AddScoped<IFacebookRepository, FacebookRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// Serviços
builder.Services.AddScoped<IApiFyService, ApiFyService>();
builder.Services.AddScoped<IPostClassifierService, PostClassifierService>();
builder.Services.AddScoped<IInstagramService, InstagramService>();
builder.Services.AddScoped<IFacebookService, FacebookService>();

builder.Services.AddScoped<INotificationProcessor, NotificationProcessor>();

// Workers
builder.Services.AddHostedService<FacebookWorker>();
builder.Services.AddHostedService<InstagramWorker>();
builder.Services.AddHostedService<NotificationWorker>();

var host = builder.Build();
host.Run();
