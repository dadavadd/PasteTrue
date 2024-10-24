using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasteTrue.DTOs.Paste;
using PasteTrue.Models;
using PasteTrue.Repositories.Interfaces;
using System.Security.Claims;

namespace PasteTrue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasteController : ApiControllerBase
    {
        private readonly IPasteRepository _pasteRepo;
        private readonly ILogger<PasteController> _logger;
        public PasteController(IPasteRepository pasteRepo, ILogger<PasteController> logger)
        {
            _pasteRepo = pasteRepo;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("getMyPastes")]
        public async Task<ActionResult<IEnumerable<PasteDto>>> GetUserPastes()
        {
            var userId = User.FindFirstValue("id");
            
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID not found in claims");

            var pastes = await _pasteRepo.GetUserPastes(userId);
            return Ok(pastes);
        }

        [HttpPost("create")]
        public async Task<ActionResult<PasteDto>> CreatePaste([FromBody] CreatePasteDto createPasteDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string userId = User.FindFirstValue("id");

            if (!createPasteDto.IsPublic && string.IsNullOrEmpty(userId))
                return Unauthorized("You must be logged in to create private pastes.");

            var paste = new Paste
            {
                Content = createPasteDto.Content,
                CreatedAt = DateTime.UtcNow,
                Title = createPasteDto.Title,
                UserId = userId,
                IsPublic = createPasteDto.IsPublic
            };

            if (!string.IsNullOrEmpty(createPasteDto.Password))
                paste.SetPassword(createPasteDto.Password);

            try
            {
                var createdPaste = await _pasteRepo.CreatePaste(paste);

                var responseDto = new PasteDto
                {
                    Id = createdPaste.Id,
                    Title = createdPaste.Title,
                    Content = createdPaste.Content,
                    CreatedAt = createdPaste.CreatedAt,
                    IsPublic = createdPaste.IsPublic
                };

                return CreatedAtAction(nameof(GetPasteById), new { id = responseDto.Id }, responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating paste");
                return StatusCode(500, "Error creating paste");
            }
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePaste(int id)
        {
            var userId = User.FindFirstValue("id");
            var paste = await _pasteRepo.GetPasteById(id);

            if (paste == null)
                return NotFound();

            if (paste.UserId != userId)
                return Forbid("You can only delete your own pastes");

            try
            {
                await _pasteRepo.DeletePaste(paste);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting paste {Id}", id);
                return StatusCode(500, "Error deleting paste");
            }
        }


        [AllowAnonymous]
        [HttpPost("getPaste/{id:int}")]
        public async Task<ActionResult<PasteDto>> GetPasteById(
            [FromRoute] int id,
            [FromQuery] string password = null)
        {
            var paste = await _pasteRepo.GetPasteById(id);
            if (paste == null)
                return NotFound("Paste not found");

            if (!paste.IsPublic)
            {
                if (!User.Identity.IsAuthenticated)
                    return Unauthorized("Authentication is required to access this paste.");

                var userId = User.FindFirstValue("id");
                if (paste.UserId != userId)
                    return Unauthorized("You are not allowed to access this paste.");
            }

            if (!paste.VerifyPassword(password))
                return Unauthorized("Invalid password for this paste.");

            return Ok(new PasteDto
            {
                Content = paste.Content,
                CreatedAt = paste.CreatedAt,
                Id = paste.Id,
                Title = paste.Title,
                IsPublic = paste.IsPublic
            });
        }
    }
}
