﻿using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebAPI_PrintingSystem.Business;
using WebAPI_PrintingSystem.DTOs;

namespace WebAPI_PrintingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly IBalanceHelper _balanceHelper;

        public BalanceController(IBalanceHelper balanceHelper)
        {
            _balanceHelper = balanceHelper;
        }


        [HttpPost("creditUIDWithQuotaCHF")]
        public async Task<ActionResult<(decimal, int, bool)>> CreditUIDWithQuotaCHF([FromBody] CreditUIDRequestDTO request)
        {
            try
            {
                if (request.UserID == Guid.Empty || request.QuotaCHF <= 0)
                {
                    return BadRequest(new { message = "Valid UserID and a positive QuotaCHF amount are required." });
                }

                var result = await _balanceHelper.creditUIDWithQuotaCHF(request.UserID, request.QuotaCHF);
                return Ok(new
                {
                    NewQuotaCHF = result.Item1,
                    NewPrintQuota = result.Item2,
                    Done = result.Item3
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while crediting the user.", error = ex.Message });
            }
        }

  
        [HttpPost("creditUsernameWithQuotaCHF")]
        public async Task<ActionResult<(decimal, bool)>> CreditUsernameWithQuotaCHF([FromBody] CreditUsernameRequestDTO request)
        {
            try
            {
               
                if (string.IsNullOrWhiteSpace(request.Username) || request.QuotaCHF <= 0)
                {
                    return BadRequest(new { message = "Username and a positive QuotaCHF amount are required." });
                }

                
                var result = await _balanceHelper.creditUsernameWithQuotaCHF(request.Username, request.QuotaCHF);

                return Ok(new
                {
                    QuotaCHFCharged = result.Item1,
                    Done = result.Item2
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while crediting the user.", error = ex.Message });
            }
        }
        [HttpPost("creditGroupWithQuotaCHF")]
        public async Task<ActionResult<(List<User>, decimal, bool)>> CreditGroupWithQuotaCHF([FromBody] CreditGroupRequestDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Group) || request.QuotaCHF <= 0)
                {
                    return BadRequest(new { message = "Group name and a positive QuotaCHF amount are required." });
                }
                var result = await _balanceHelper.creditGroupWithQuotaCHF(request.Group, request.QuotaCHF);
                return Ok(new
                {
                    Users = result.Item1,
                    QuotaCHFCharged = result.Item2,
                    Done = result.Item3
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while crediting the group.", error = ex.Message });
            }
        }

    }
    
}
