using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBA.Business;
using TBA.Models.DTOs;
using TBA.Models.Entities;

namespace TBA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly IBusinessComment _businessComment;

        public CommentController(IBusinessComment businessComment)
        {
            _businessComment = businessComment;
        }

        // GET api/comment
        // Devuelve todos los comentarios
        [HttpGet]
        public async Task<ActionResult<List<CommentViewModel>>> GetAll()
        {
            var comments = await _businessComment.GetAllCommentsAsync();
            var vms = comments.Select(c => new CommentViewModel
            {
                CommentID   = c.CommentId,
                CardID      = c.CardId ?? 0,
                UserID      = c.UserId ?? 0,
                CommentText = c.CommentText ?? string.Empty,
                CreatedAt   = c.CreatedAt ?? DateTime.MinValue,
                Username    = c.User?.Username ?? c.CreatedBy ?? "Anon"
            }).ToList();

            return Ok(vms);
        }

        // GET api/comment/card/5
        // Lista los comentarios de la tarjeta con ID = cardId
        [HttpGet("card/{cardId}")]
        public async Task<ActionResult<List<CommentViewModel>>> GetByCard(int cardId)
        {
            var comments = await _businessComment.GetAllCommentsAsync();
            var filtered = comments
                .Where(c => c.CardId == cardId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentViewModel
                {
                    CommentID   = c.CommentId,
                    CardID      = c.CardId ?? 0,
                    UserID      = c.UserId ?? 0,
                    CommentText = c.CommentText ?? string.Empty,
                    CreatedAt   = c.CreatedAt ?? DateTime.MinValue,
                    Username    = c.User?.Username ?? c.CreatedBy ?? "Anon"
                })
                .ToList();

            return Ok(filtered);
        }

        // GET api/comment/detail/10
        // Obtiene un único comentario por su ID
        [HttpGet("detail/{id}")]
        public async Task<ActionResult<CommentViewModel>> GetById(int id)
        {
            var c = await _businessComment.GetCommentAsync(id);
            if (c == null) return NotFound();

            var vm = new CommentViewModel
            {
                CommentID   = c.CommentId,
                CardID      = c.CardId ?? 0,
                UserID      = c.UserId ?? 0,
                CommentText = c.CommentText ?? string.Empty,
                CreatedAt   = c.CreatedAt ?? DateTime.MinValue,
                Username    = c.User?.Username ?? c.CreatedBy ?? "Anon"
            };
            return Ok(vm);
        }

        // POST api/comment
        // Crea un nuevo comentario
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CommentViewModel vm)
        {
            if (vm == null || string.IsNullOrWhiteSpace(vm.CommentText))
                return BadRequest("Comentario vacío");

            var entity = new Comment
            {
                CardId      = vm.CardID,
                UserId      = vm.UserID,
                CommentText = vm.CommentText,
                CreatedAt   = vm.CreatedAt == default
                              ? DateTime.Now
                              : vm.CreatedAt
            };

            var success = await _businessComment.SaveCommentAsync(entity);
            if (!success)
                return StatusCode(500, "No se pudo guardar el comentario");

            vm.CommentID = entity.CommentId;
            return CreatedAtAction(nameof(GetById),
                                   new { id = vm.CommentID },
                                   vm);
        }

        // DELETE api/comment/12
        // Elimina el comentario con ID = id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _businessComment.GetCommentAsync(id);
            if (c == null) return NotFound();

            var success = await _businessComment.DeleteCommentAsync(c);
            return success
                ? NoContent()
                : StatusCode(500, "No se pudo eliminar el comentario");
        }
    }
}
