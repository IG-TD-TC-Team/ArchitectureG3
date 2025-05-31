using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI_PrintingSystem.Business;
using WebAPI_PrintingSystem.DTOs;

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
        public async Task<ActionResult> AuthenticateByCard([FromBody] CardAuthenticationRequestDTO request)
        {
            try
            {
                // Validate input 
                if (request.CardID == Guid.Empty)
                {
                    return BadRequest(new { message = "Card ID is required." });
                }

                try
                {
                    var result = await _authentificationHelper.authenticateByCard(request.CardID);

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
                // Log the exception
                return StatusCode(500, new { message = "An error occurred during authentication.", error = ex.Message });
            }
        }


        [HttpPost("authenticateByUsername")]
        public async Task<ActionResult> AuthenticateByUsername([FromBody] AuthenticationRequestDTO request)
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
                        return Ok(new
                        {
                            message = result.Item1,
                            UID = result.Item2,
                            isStaff = result.Item3,
                            group = result.Item4 
                        });
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




}