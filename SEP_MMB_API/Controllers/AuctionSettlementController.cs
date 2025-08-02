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

        [Authorize]
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

    }
}
