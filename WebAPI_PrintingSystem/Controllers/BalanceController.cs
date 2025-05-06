using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI_PrintingSystem.Business;

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
        public async Task<ActionResult<(decimal, int, bool)>> CreditUIDWithQuotaCHF(Guid userID, decimal quotaCHF)
        {
            try
            {
                var result = await _balanceHelper.creditUIDWithQuotaCHF(userID, quotaCHF);
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
        public async Task<ActionResult<(decimal, bool)>> CreditUsernameWithQuotaCHF(string username, decimal quotaCHF)
        {
            try
            {
                var result = await _balanceHelper.creditUsernameWithQuotaCHF(username, quotaCHF);
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

    }
    
}
