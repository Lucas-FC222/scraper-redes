using Core;
using Infra;
using Infra.Data;
using Services;
using Microsoft.Data.SqlClient;

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

// Serviços
builder.Services.AddScoped<IApiFyService, ApiFyService>();
builder.Services.AddScoped<IInstagramService, InstagramService>();
builder.Services.AddScoped<IFacebookService, FacebookService>();

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
