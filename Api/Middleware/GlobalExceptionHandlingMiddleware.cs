using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middleware
{
    /// <summary>
    /// Middleware para tratamento global de exceções
    /// </summary>
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Inicializa uma nova instância do middleware de tratamento de exceções
        /// </summary>
        /// <param name="next">O próximo middleware no pipeline</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Processa uma requisição HTTP e captura exceções não tratadas
        /// </summary>
        /// <param name="context">O contexto HTTP da requisição</param>
        /// <returns>Tarefa que representa a operação assíncrona</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Processa exceções e cria uma resposta HTTP apropriada
        /// </summary>
        /// <param name="context">O contexto HTTP da requisição</param>
        /// <param name="exception">A exceção que foi capturada</param>
        /// <returns>Tarefa que representa a operação assíncrona</returns>
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Erro não tratado: {ExceptionMessage}", exception.Message);

            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = "Ocorreu um erro interno no servidor.";

            // Determinar o código de status HTTP baseado no tipo de exceção
            if (exception is KeyNotFoundException || exception is FileNotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
                message = "O recurso solicitado não foi encontrado.";
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized;
                message = "Acesso não autorizado.";
            }
            else if (exception is ArgumentException || exception is FormatException)
            {
                statusCode = HttpStatusCode.BadRequest;
                message = "Requisição inválida.";
            }

            // Criar resposta de erro
            var problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = message,
                Detail = exception.Message,
                Instance = context.Request.Path,
                Type = $"https://httpstatuses.com/{(int)statusCode}"
            };

            // Configurar resposta
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)statusCode;

            // Criar um resultado de erro usando o padrão Result
            var errorResult = new { success = false, data = (object?)null, error = problemDetails };
            
            // Serializar e retornar resposta
            var result = JsonSerializer.Serialize(errorResult);
            return context.Response.WriteAsync(result);
        }
    }

    /// <summary>
    /// Extensão para registrar o middleware de tratamento global de exceções
    /// </summary>
    public static class GlobalExceptionHandlingMiddlewareExtensions
    {
        /// <summary>
        /// Adiciona o middleware de tratamento global de exceções ao pipeline da aplicação
        /// </summary>
        /// <param name="app">O construtor de aplicação</param>
        /// <returns>A referência ao construtor de aplicação para encadeamento</returns>
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        }
    }
}
