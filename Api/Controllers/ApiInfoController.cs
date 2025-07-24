using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.V1
{
    /// <summary>
    /// Controller de versão demonstrativa - Versão 1
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/versao")]
    [ApiController]
    public class ApiInfoController : ApiControllerBase
    {
        /// <summary>
        /// Retorna informações da versão atual da API (V1)
        /// </summary>
        /// <returns>Informações sobre a versão da API</returns>
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

namespace Api.Controllers.V2
{
    /// <summary>
    /// Controller de versão demonstrativa - Versão 2 (Futura)
    /// </summary>
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/versao")]
    [ApiController]
    public class ApiInfoController : ApiControllerBase
    {
        /// <summary>
        /// Retorna informações da versão atual da API (V2)
        /// </summary>
        /// <returns>Informações sobre a versão da API</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok(new
            {
                version = "2.0",
                status = "Em desenvolvimento",
                features = new[]
                {
                    "Autenticação por JWT",
                    "Scraping de Instagram (aprimorado)",
                    "Scraping de Facebook (aprimorado)",
                    "Scraping de Twitter",
                    "Notificações em tempo real",
                    "Análise de sentimento"
                },
                releaseDate = "Previsto para dezembro/2025"
            });
        }
    }
}
