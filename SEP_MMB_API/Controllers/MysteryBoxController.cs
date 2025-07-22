using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MysteryBoxController : ControllerBase
    {
        private readonly IMysteryBoxService _mysteryBoxService;

        public MysteryBoxController(IMysteryBoxService mysteryBoxService)
        {
            _mysteryBoxService = mysteryBoxService;
        }

        [Authorize]
        [HttpGet("box-image-paths")]
        public async Task<ActionResult<ResponseModel<List<string>>>> GetUniqueBoxImagePaths()
        {
            try
            {
                var result = await _mysteryBoxService.GetAllUniqueImageUrlsAsync();

                return Ok(new ResponseModel<List<string>>
                {
                    Data = result,
                    Success = true,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<List<string>>
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
