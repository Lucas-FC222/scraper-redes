using Microsoft.AspNetCore.Mvc;

namespace Shared.Domain.Models
{
    /// <summary>
    /// Representa um resultado de operação padronizado para APIs
    /// </summary>
    /// <typeparam name="T">Tipo do dado retornado em caso de sucesso</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Indica se a operação foi bem-sucedida
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// Dados retornados em caso de sucesso
        /// </summary>
        public T? Data { get; private set; }

        /// <summary>
        /// Detalhes do problema em caso de erro
        /// </summary>
        public ProblemDetails? Error { get; private set; }

        private Result(bool success, T? data, ProblemDetails? error)
        {
            Success = success;
            Data = data;
            Error = error;
        }

        /// <summary>
        /// Cria um resultado de sucesso com dados
        /// </summary>
        /// <param name="data">Dados do resultado</param>
        /// <returns>Um resultado de sucesso</returns>
        public static Result<T> Ok(T data)
        {
            return new Result<T>(true, data, null);
        }

        /// <summary>
        /// Cria um resultado de erro com detalhes do problema
        /// </summary>
        /// <param name="problemDetails">Detalhes do problema</param>
        /// <returns>Um resultado de erro</returns>
        public static Result<T> Fail(ProblemDetails problemDetails)
        {
            return new Result<T>(false, default, problemDetails);
        }


        /// <summary>
        /// Converte este resultado em um ActionResult para uso em controllers
        /// </summary>
        /// <returns>ActionResult apropriado baseado no resultado</returns>
        public IActionResult ToActionResult()
        {
            if (Success)
            {
                return new OkObjectResult(this);
            }
            
            return new ObjectResult(this)
            {
                StatusCode = Error?.Status ?? 500
            };
        }
    }

    /// <summary>
    /// Métodos de extensão e ajudantes para Result
    /// </summary>
    public static class Result
    {
        /// <summary>
        /// Cria um resultado de sucesso sem dados
        /// </summary>
        /// <returns>Um resultado de sucesso</returns>
        public static Result<object> Ok()
        {
            return Result<object>.Ok(new { });
        }

        /// <summary>
        /// Cria um resultado de erro com detalhes específicos
        /// </summary>
        /// <param name="statusCode">Código de status HTTP</param>
        /// <param name="title">Título do erro</param>
        /// <param name="detail">Detalhes do erro</param>
        /// <param name="instance">Caminho da instância onde ocorreu o erro</param>
        /// <returns>Um resultado de erro</returns>
        public static Result<T> Fail<T>(int statusCode, string title, string detail, string instance)
        {
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = instance
            };
            return Result<T>.Fail(problemDetails);
        }
    }
}
