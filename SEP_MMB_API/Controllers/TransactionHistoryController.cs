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
        [Authorize]
        [HttpGet("withdraw-transaction-request")]
        public async Task<ActionResult<ResponseModel<List<TransactionHistoryRequestWithdrawOfUserDto>>>> GetRequestWithdrawTransaction()
        {
            try
            {
                var transactions = await _transactionHistoryService.GetAllRequestWithdrawAsync();

                return Ok(new ResponseModel<List<TransactionHistoryRequestWithdrawOfUserDto>>
                {
                    Data = transactions,
                    Success = true,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<List<TransactionHistoryRequestWithdrawOfUserDto>>
                {
                    Data = null,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }
        [Authorize(Roles ="user")]
        [HttpPost("create-withdraw-transaction-request")]
        public async Task<ActionResult<ResponseModel<List<TransactionHistoryRequestWithdrawOfUserDto>>>> CreateRequestWithdrawTransaction(int amount)
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var transaction = await _transactionHistoryService.CreateRequestWithdrawAsync(account.Id,amount);
                if (transaction == null)
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        Data = null,
                        Success = false,
                        Error = "User does not have a bank account",
                        ErrorCode = 400
                    });
                }
                if (transaction is WithdrawLimitInfoDto limitInfo)
                {
                    return BadRequest(new ResponseModel<WithdrawLimitInfoDto>
                    {
                        Data = limitInfo,
                        Success = false,
                        Error = limitInfo.Message,
                        ErrorCode = 400
                    });
                }

                return Ok(new ResponseModel<string>
                {
                    Data = "Create request withdraw successfull ",
                    Success = true,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
                {
                    Data = null,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }
        [Authorize]
        [HttpPatch("accept-withdraw-transaction-request")]
        public async Task<ActionResult<ResponseModel<List<TransactionHistoryRequestWithdrawOfUserDto>>>> AcceptRequestWithdrawTransaction(string transactionId, string transactionCode)
        {
            try
            {
                var transaction = await _transactionHistoryService.AcceptTransactionWithdrawAsync(transactionId,transactionCode);
                if (transaction == false)
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        Data = null,
                        Success = false,
                        Error = "Accpet failed",
                        ErrorCode = 400
                    });
                }
                return Ok(new ResponseModel<string>
                {
                    Data = "Accepting successfull ",
                    Success = true,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
                {
                    Data = null,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }
        [Authorize]
        [HttpPatch("reject-withdraw-transaction-request")]
        public async Task<ActionResult<ResponseModel<List<TransactionHistoryRequestWithdrawOfUserDto>>>> RejectRequestWithdrawTransaction(string transactionId)
        {
            try
            {
                var transaction = await _transactionHistoryService.RejectTransactionWithdrawAsync(transactionId);
                if (transaction == false)
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        Data = null,
                        Success = false,
                        Error = "Reject failed",
                        ErrorCode = 400
                    });
                }
                return Ok(new ResponseModel<string>
                {
                    Data = "Reject successfull ",
                    Success = true,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
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
