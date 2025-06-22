using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.TransactionHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionHistoryController : ControllerBase
    {
        private readonly ITransactionHistoryService _transactionHistoryService;
        private readonly IAuthService _authService;

        public TransactionHistoryController(ITransactionHistoryService transactionHistoryService, IAuthService authService)
        {
            _transactionHistoryService = transactionHistoryService;
            _authService = authService;
        }

        [Authorize(Roles = "user")]
        [HttpGet("transaction-history")]
        public async Task<ActionResult<ResponseModel<List<TransactionHistoryDto>>>> GetUserWalletTransactions()
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var walletId = account.WalletId;

                var transactions = await _transactionHistoryService.GetTransactionHistoryAsync(walletId);

                return Ok(new ResponseModel<List<TransactionHistoryDto>>
                {
                    Data = transactions,
                    Success = true,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<List<TransactionHistoryDto>>
                {
                    Data = null,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }
    }
}
