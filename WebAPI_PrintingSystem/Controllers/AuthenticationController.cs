using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI_PrintingSystem.Business;

namespace WebAPI_PrintingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthentificationHelper _authentificationHelper;

        public AuthenticationController(IAuthentificationHelper authentificationHelper)
        {
            _authentificationHelper = authentificationHelper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _authentificationHelper.GetUsers();
            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }
            return Ok(users);
        }

        [HttpPost("authenticateByCard")]
        public async Task<ActionResult> AuthenticateByCard([FromBody] Guid cardId)
        {
            try
            {
                var result = await _authentificationHelper.authenticateByCard(cardId);

                // Check if authentication was successful
                if (result.Item1.ToLower().Contains("successfull access"))
                {
                    // Return success with both the message and the UID
                    return Ok(new
                    {
                        message = result.Item1,
                        UID = result.Item2
                    });
                }
                else
                {
                    // Return unauthorized with error message
                    return Unauthorized(new
                    {
                        message = result.Item1
                    });
                }
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging in place
                return StatusCode(500, new { message = "An error occurred during authentication.", error = ex.Message });
            }
        }

        [HttpPost("authenticateByUsername")]
        public async Task<ActionResult> AuthenticateByUsername([FromBody] AuthenticationRequest request)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { message = "Username and password are required." });
                }

                var result = await _authentificationHelper.authenticateByUsername(request.Username, request.Password);

                // Check if authentication was successful
                if (result.Equals("Successfull access", StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(new { message = result });
                }
                else
                {
                    return Unauthorized(new { message = result });
                }
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging in place
                return StatusCode(500, new { message = "An error occurred during authentication.", error = ex.Message });
            }
        }
    }

    // DTO for username/password authentication
    public class AuthenticationRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}