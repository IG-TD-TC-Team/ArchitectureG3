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

        [HttpGet("checkUsername")]
        public async Task<ActionResult<Guid>> checkUsername(string username)
        {
            try
            {

                bool result = await _authentificationHelper.checkUsername(username);
                return Ok(new { exists = result});
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking the username.", error = ex.Message });
            }
        }

        [HttpPost("authenticateByCard")]
        public async Task<ActionResult> AuthenticateByCard([FromBody] CardAuthenticationRequest request)
        {
            try
            {
                // Validate input 
                if (request.cardID == Guid.Empty)
                {
                    return BadRequest(new { message = "Card ID is required." });
                }

                try
                {
                    var result = await _authentificationHelper.authenticateByCard(request.cardID);

                    // Check if authentication was successful
                    if (result.Item1.ToLower().Contains("successful access"))
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
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { message = "An error occurred during authentication.", error = ex.Message });
                }
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging in place
                return StatusCode(500, new { message = "An error occurred during authentication.", error = ex.Message });
            }
        }

        // DTO for card authentication
        public class CardAuthenticationRequest
        {
            public Guid cardID { get; set; }
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

                try
                {
                    var result = await _authentificationHelper.authenticateByUsernameWithStaffCheck(request.Username, request.Password);

                    // Check if authentication was successful
                    if (result.Item1.ToLower().Contains("successful access"))
                    {
                        return Ok(new { message = result.Item1, UID = result.Item2, isStaff = result.Item3 });
                    }
                    else
                    {
                        return Unauthorized(new { message = result.Item1 });
                    }
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { message = "An error occurred during authentication.", error = ex.Message });
                }
            }
            catch (Exception ex)
            {
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