using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.TransactionFee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionFeeController(ITransactionFeeService transactionFeeService) : ControllerBase
    {
        [Authorize(Roles = "admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var data = await transactionFeeService.GetAllValidTransactionFeesAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Data = null,
                        Error = "Don't have any transaction fee",
                        ErrorCode = 404
                    });
                }

                return Ok(new ResponseModel<List<TransactionFeeDto>>
                {
                    Success = true,
                    Data = data,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Data = null,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }
    }
}
