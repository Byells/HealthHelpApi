using System.Security.Claims;
using HealthHelp.Api.Dtos;
using HealthHelp.Api.Services; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Asp.Versioning;

namespace HealthHelp.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    [Produces("application/json")]
    [SwaggerTag("Realiza diagnósticos inteligentes sobre a rotina do usuário")]
    public class AnalysisController : ControllerBase
    {
        private readonly IRoutineAnalyzer _analyzer;

        public AnalysisController(IRoutineAnalyzer analyzer)
        {
            _analyzer = analyzer;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Gerar diagnóstico detalhado (Sistema Especialista)",
            Description = "Utiliza um motor de regras heurísticas ponderadas para analisar Sono, Trabalho, Lazer e Atividade. Retorna scores por categoria, dicas personalizadas e uma nota geral de 0 a 100."
        )]
        [SwaggerResponse(200, "Análise gerada com sucesso", typeof(AnalysisDto))]
        [SwaggerResponse(401, "Não autorizado")]
        public async Task<IActionResult> GetAnalysis()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized();

            var analysis = await _analyzer.AnalyzeForUserAsync(userId);

            return Ok(analysis);
        }
    }
}