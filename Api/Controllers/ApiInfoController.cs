using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    /// <summary>
    /// Controller responsável por fornecer informações sobre a versão e recursos da API.
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v1/info")]
    public class ApiInfoController : ApiControllerBase
    {
        /// <summary>
        /// Inicializa uma nova instância de <see cref="ApiInfoController"/>.
        /// </summary>
        /// <param name="mediator">Instância do MediatR injetada.</param>
        public ApiInfoController(IMediator mediator)
            : base(mediator)
        {
        }

        /// <summary>
        /// Obtém informações sobre a versão da API e funcionalidades disponíveis.
        /// </summary>
        /// <returns>Objeto anônimo contendo versão, status e lista de funcionalidades.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok(new
            {
                version = "1.0",
                status = "Versão estável",
                features = new[]
                {
                    "Autenticação por JWT",
                    "Scraping de Instagram",
                    "Scraping de Facebook",
                    "Notificações"
                }
            });
        }
    }
}
