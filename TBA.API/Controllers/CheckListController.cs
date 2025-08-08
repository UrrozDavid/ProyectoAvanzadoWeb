using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using TBA.API.Hubs;
using TBA.Models.DTOs;
using TBA.Models.Entities;
using TBA.Services;

[ApiController]
[Route("api/[controller]")]
public class ChecklistController : ControllerBase
{
    private readonly IChecklistService _service;
    private readonly IHubContext<NotificationHub> _hubContext;

    public ChecklistController(IChecklistService service, IHubContext<NotificationHub> hubContext)
    {
        _service = service;
        _hubContext = hubContext;
    }

    // GET api/checklist/5
    [HttpGet("{cardId}")]
    public async Task<ActionResult<List<ChecklistItemDto>>> GetByCard(int cardId)
    {
        var items = await _service.GetItemsByCardIdAsync(cardId);
        var dtos = items.Select(i => new ChecklistItemDto
        {
            ChecklistItemId = i.ChecklistItemId,
            CardId          = i.CardId,
            Text            = i.Text,
            IsDone          = i.IsDone,
            Position        = i.Position
        }).ToList();
        return Ok(dtos);
    }

    // POST api/checklist
    [HttpPost]
    public async Task<ActionResult<ChecklistItemDto>> Add([FromBody] ChecklistItemDto dto)
    {
        try
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Text) || dto.CardId <= 0)
                return BadRequest("Datos inválidos.");

            var entity = new ChecklistItem
            {
                CardId   = dto.CardId,
                Text     = dto.Text,
                IsDone   = false,
                Position = dto.Position
            };
            var created = await _service.AddItemAsync(entity);
            dto.ChecklistItemId = created.ChecklistItemId;
            dto.IsDone          = created.IsDone;
            await _hubContext.Clients.All.SendAsync("ReceiveNotification",
        $"Nueva subtarea agregada: {dto.Text}");
            return CreatedAtAction(nameof(GetByCard),
                                   new { cardId = dto.CardId },
                                   dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    // PUT api/checklist/3/toggle
    [HttpPut("{id}/toggle")]
    public async Task<IActionResult> Toggle(int id, [FromQuery] bool isDone)
    {
        var ok = await _service.ToggleItemAsync(id, isDone);
        return ok ? NoContent() : NotFound();
    }

    // DELETE api/checklist/3
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
        => await _service.DeleteItemAsync(id)
            ? NoContent()
            : NotFound();
}
