using Microsoft.AspNetCore.Mvc;

namespace DevOpsAIAgent.API.Controllers
{
    /// <summary>
    /// Base controller for all API controllers
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseApiController : ControllerBase
    {
        protected ILogger Logger { get; }

        protected BaseApiController(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Returns a standardized success response
        /// </summary>
        protected IActionResult SuccessResponse<T>(T data, string? message = null)
        {
            return Ok(new
            {
                Success = true,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Returns a standardized error response
        /// </summary>
        protected IActionResult ErrorResponse(string message, int statusCode = 400)
        {
            return StatusCode(statusCode, new
            {
                Success = false,
                Message = message,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}