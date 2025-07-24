using Core;
using Data;
using Infra;
using Infra.Data;
using Microsoft.Data.SqlClient;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Configurações do Apify
builder.Services.Configure<ApifySettings>(builder.Configuration.GetSection("Apify"));

// Configurações do Youtube
builder.Services.Configure<YouTubeSettings>(builder.Configuration.GetSection("YouTube"));

// Configurações do Groq
builder.Services.Configure<GroqSettings>(builder.Configuration.GetSection("Groq"));
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
builder.Services.AddScoped<IYouTubeVideoRepository, YouTubeVideoRepository>();

// Serviços
builder.Services.AddScoped<IApiFyService, ApiFyService>();
builder.Services.AddScoped<IPostClassifierService, PostClassifierService>();
builder.Services.AddScoped<IInstagramService, InstagramService>();
builder.Services.AddScoped<IFacebookService, FacebookService>();
builder.Services.AddScoped<IYouTubeAnalyzerService, YouTubeAnalyzerService>();
builder.Services.AddScoped<INotificationProcessor, NotificationProcessor>();

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

// Controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Servir arquivos estáticos do wwwroot

// Usar CORS
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();
