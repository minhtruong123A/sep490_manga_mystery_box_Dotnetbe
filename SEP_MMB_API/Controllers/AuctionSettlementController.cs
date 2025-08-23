using BusinessObjects;
using BusinessObjects.Dtos.Auction;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionSettlementController : ControllerBase
    {
        private readonly IAuctionSettlementService _auctionSettlementService;

        public AuctionSettlementController(IAuctionSettlementService auctionSettlementService)
        {
            _auctionSettlementService = auctionSettlementService;
        }

        [HttpPost("finalize-auction/{auctionId}")]
        public async Task<ActionResult<ResponseModel<object>>> FinalizeAuction(string auctionId)
        {
            try
            {
                var result = await _auctionSettlementService.FinalizeAuctionResultAsync(auctionId);

                return Ok(new ResponseModel<object>
                {
                    Data = result,
                    Success = true,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<object>
                {
                    Data = false,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }

        [Authorize]
        [HttpPatch("update-status-auction-session")]
        public async Task<ActionResult<ResponseModel<object>>> UpdadteStatusAuctionSession(string auctionSessionId, int status)
        {
            try
            {
                var result = await _auctionSettlementService.ChangeStatusAsync(auctionSessionId,status);
                if (status == 1 && result==true)
                {
                    return Ok(new ResponseModel<object>
                    {
                        Data = "Approve auction successful",
                        Success = true,
                        Error = null,
                        ErrorCode = 0
                    });
                }
                if(status == -1 && result==true)
                {
                    return Ok(new ResponseModel<object>
                    {
                        Data = "Reject auction successful",
                        Success = true,
                        Error = null,
                        ErrorCode = 0
                    });
                }
                return BadRequest(new ResponseModel<object>
                {
                    Data = false,
                    Success = false,
                    Error = "Fail to change status auction session",
                    ErrorCode = 400
                });
            }
            catch (Exception ex) 
            {
                return BadRequest(new ResponseModel<object>
                {
                    Data = false,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            try
            {
                var result = await _auctionSettlementService.GetAuctionResultByIdAsync(id);
                if (result == null)
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Data = null,
                        Error = "Auction result not found",
                        ErrorCode = 404
                    });

                return Ok(new ResponseModel<AuctionResultDto>
                {
                    Success = true,
                    Data = result,
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
