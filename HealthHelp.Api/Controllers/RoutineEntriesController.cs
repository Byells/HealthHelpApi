using System.Security.Claims;
using Asp.Versioning;
using HealthHelp.Api.Data;
using HealthHelp.Api.Dtos;
using HealthHelp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace HealthHelp.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    [Produces("application/json")]
    [SwaggerTag("Gerencia as atividades da rotina do usuário (Sono, Trabalho, Lazer)")]
    public class RoutineEntriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoutineEntriesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Listar rotinas",
            Description = "Retorna uma lista paginada das atividades diárias do usuário logado, ordenadas da mais recente para a mais antiga. Inclui links HATEOAS para cada item."
        )]
        [SwaggerResponse(200, "Lista de rotinas recuperada com sucesso", typeof(PagedResultDto<RoutineEntryDto>))]
        [SwaggerResponse(401, "Não autorizado (Token JWT ausente ou inválido)")]
        public async Task<IActionResult> GetRoutineEntries([FromQuery] PaginationParams paginationParams)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _context.RoutineEntries
                .Where(entry => entry.ApplicationUserId == userId)
                .OrderByDescending(entry => entry.EntryDate);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            var routineDtos = items.Select(entry =>
            {
                var dto = new RoutineEntryDto
                {
                    Id = entry.Id,
                    Category = entry.Category,
                    Description = entry.Description,
                    Hours = entry.Hours,
                    EntryDate = entry.EntryDate
                };

                dto.Links.Add(new LinkDto(Url.Action(nameof(GetRoutineEntry), new { id = entry.Id }), "self", "GET"));
                dto.Links.Add(new LinkDto(Url.Action(nameof(UpdateRoutineEntry), new { id = entry.Id }), "update", "PUT"));
                dto.Links.Add(new LinkDto(Url.Action(nameof(DeleteRoutineEntry), new { id = entry.Id }), "delete", "DELETE"));

                return dto;
            }).ToList();

            var pagedResult = new PagedResultDto<RoutineEntryDto>(
                routineDtos,
                totalCount,
                paginationParams.PageNumber,
                paginationParams.PageSize
            );

            return Ok(pagedResult);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Obter rotina por ID",
            Description = "Busca os detalhes de um registro de rotina específico pelo seu ID."
        )]
        [SwaggerResponse(200, "Registro encontrado com sucesso", typeof(RoutineEntryDto))]
        [SwaggerResponse(404, "Registro não encontrado ou não pertence ao usuário")]
        [SwaggerResponse(401, "Não autorizado")]
        public async Task<IActionResult> GetRoutineEntry(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var entry = await _context.RoutineEntries
                .FirstOrDefaultAsync(e => e.Id == id && e.ApplicationUserId == userId);

            if (entry == null)
            {
                return NotFound(new { Message = "Registro não encontrado ou não pertence a você." });
            }

            var dto = new RoutineEntryDto
            {
                Id = entry.Id,
                Category = entry.Category,
                Description = entry.Description,
                Hours = entry.Hours,
                EntryDate = entry.EntryDate
            };

            dto.Links.Add(new LinkDto(Url.Action(nameof(GetRoutineEntry), new { id }), "self", "GET"));
            dto.Links.Add(new LinkDto(Url.Action(nameof(UpdateRoutineEntry), new { id }), "update", "PUT"));
            dto.Links.Add(new LinkDto(Url.Action(nameof(DeleteRoutineEntry), new { id }), "delete", "DELETE"));

            return Ok(dto);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Criar nova rotina",
            Description = "Adiciona um novo registro de atividade (ex: 8 horas de sono, 2 horas de lazer) à rotina do usuário."
        )]
        [SwaggerResponse(201, "Registro criado com sucesso", typeof(RoutineEntryDto))]
        [SwaggerResponse(400, "Dados inválidos (ex: campos obrigatórios faltando)")]
        [SwaggerResponse(401, "Não autorizado")]
        public async Task<IActionResult> CreateRoutineEntry([FromBody] CreateRoutineEntryDto createDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var entry = new RoutineEntry
            {
                Category = createDto.Category,
                Description = createDto.Description,
                Hours = createDto.Hours,
                EntryDate = createDto.EntryDate,
                ApplicationUserId = userId
            };

            _context.RoutineEntries.Add(entry);
            await _context.SaveChangesAsync();

            var dto = new RoutineEntryDto
            {
                Id = entry.Id,
                Category = entry.Category,
                Description = entry.Description,
                Hours = entry.Hours,
                EntryDate = entry.EntryDate
            };

            dto.Links.Add(new LinkDto(Url.Action(nameof(GetRoutineEntry), new { id = entry.Id }), "self", "GET"));
            dto.Links.Add(new LinkDto(Url.Action(nameof(UpdateRoutineEntry), new { id = entry.Id }), "update", "PUT"));
            dto.Links.Add(new LinkDto(Url.Action(nameof(DeleteRoutineEntry), new { id = entry.Id }), "delete", "DELETE"));

            return CreatedAtAction(nameof(GetRoutineEntry), new { id = entry.Id }, dto);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Atualizar rotina",
            Description = "Atualiza os dados (Categoria, Horas, Descrição) de um registro existente."
        )]
        [SwaggerResponse(200, "Registro atualizado com sucesso")]
        [SwaggerResponse(400, "Dados inválidos")]
        [SwaggerResponse(404, "Registro não encontrado")]
        [SwaggerResponse(401, "Não autorizado")]
        public async Task<IActionResult> UpdateRoutineEntry(int id, [FromBody] CreateRoutineEntryDto updateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var entry = await _context.RoutineEntries
                .FirstOrDefaultAsync(e => e.Id == id && e.ApplicationUserId == userId);

            if (entry == null)
            {
                return NotFound(new { Message = "Registro não encontrado ou não pertence a você." });
            }

            entry.Category = updateDto.Category;
            entry.Description = updateDto.Description;
            entry.Hours = updateDto.Hours;
            entry.EntryDate = updateDto.EntryDate;

            _context.RoutineEntries.Update(entry);
            await _context.SaveChangesAsync();

            var response = new
            {
                Message = "Registro atualizado com sucesso.",
                Links = new List<LinkDto>
                {
                    new LinkDto(Url.Action(nameof(GetRoutineEntry), new { id }), "self", "GET"),
                    new LinkDto(Url.Action(nameof(DeleteRoutineEntry), new { id }), "delete", "DELETE")
                }
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Excluir rotina",
            Description = "Remove permanentemente um registro de rotina do banco de dados."
        )]
        [SwaggerResponse(200, "Registro excluído com sucesso")]
        [SwaggerResponse(404, "Registro não encontrado")]
        [SwaggerResponse(401, "Não autorizado")]
        public async Task<IActionResult> DeleteRoutineEntry(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var entry = await _context.RoutineEntries
                .FirstOrDefaultAsync(e => e.Id == id && e.ApplicationUserId == userId);

            if (entry == null)
            {
                return NotFound(new { Message = "Registro não encontrado ou não pertence a você." });
            }

            _context.RoutineEntries.Remove(entry);
            await _context.SaveChangesAsync();

            var response = new
            {
                Message = "Registro deletado com sucesso.",
                Links = new List<LinkDto>
                {
                    new LinkDto(Url.Action(nameof(GetRoutineEntries)), "list", "GET"),
                    new LinkDto(Url.Action(nameof(CreateRoutineEntry)), "create", "POST")
                }
            };

            return Ok(response);
        }
    }
}
