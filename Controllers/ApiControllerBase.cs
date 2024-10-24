using Microsoft.AspNetCore.Mvc;

namespace PasteTrue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected ActionResult HandleError(Exception ex, string message)
        {
            return StatusCode(500, new { message = "An error occurred", details = message });
        }
    }
}
