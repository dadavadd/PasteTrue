using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasteTrue.DTOs.Paste;
using PasteTrue.Models;
using PasteTrue.Repositories.Interfaces;

namespace PasteTrue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasteController : ControllerBase
    {
        private readonly IPasteRepository _pasteRepo;
        public PasteController(IPasteRepository pasteRepo)
        {
            _pasteRepo = pasteRepo;
        }

        [Authorize]
        [HttpGet("getMyPastes")]
        public async Task<IActionResult> GetUserPastes()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id").Value;
            var pastes = await _pasteRepo.GetUserPastes(userId);

            if (pastes == null)
                return BadRequest();

            return Ok(pastes);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePaste([FromBody] CreatePasteDto createPasteDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string userId = null;

            if (User.Identity.IsAuthenticated)
                userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            var paste = new Paste
            {
                Content = createPasteDto.Content,
                CreatedAt = DateTime.UtcNow,
                Title = createPasteDto.Title,
                UserId = userId
            };

            if (!string.IsNullOrEmpty(createPasteDto.Password))
                paste.SetPassword(createPasteDto.Password);

            var createdPaste = await _pasteRepo.CreatePaste(paste);

            var responseDto = new PasteDto
            {
                Id = createdPaste.Id,
                Title = createdPaste.Title,
                Content = createdPaste.Content,
                CreatedAt = createdPaste.CreatedAt,
            };

            return CreatedAtAction(nameof(GetPasteById), new { id = responseDto.Id }, responseDto);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePaste(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var paste = await _pasteRepo.GetPasteById(id);

            if (paste == null)
                return NotFound();
            
            await _pasteRepo.DeletePaste(paste);

            return Ok("Paste deleted succesfully");
        }


        [AllowAnonymous]
        [HttpPost("getPaste/{id:int}")]
        public async Task<IActionResult> GetPasteById([FromRoute] int id, [FromQuery] string password = null)
        {
            var paste = await _pasteRepo.GetPasteById(id);

            if (paste == null)
                return NotFound("Paste not found");


            if (!paste.VerifyPassword(password))
                return Unauthorized("Invalid password for this paste.");

            var pasteDto = new PasteDto
            {
                Content = paste.Content,
                CreatedAt = paste.CreatedAt,
                Id = paste.Id,
                Title = paste.Title,
            };

            return Ok(pasteDto);
        }
    }
}
