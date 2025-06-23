using BusinessObjects.Dtos.MangaBox;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MangaBoxController : ControllerBase
    {
        private readonly IMangaBoxService _mangaBoxService;

        public MangaBoxController(IMangaBoxService mangaBoxService)
        {
            _mangaBoxService = mangaBoxService;
        }

        [HttpGet("get-all-mystery-box")]
        public async Task<ActionResult<ResponseModel<List<MangaBoxGetAllDto>>>> GetMangaBoxDetails()
        {
            try
            {
                var response = await _mangaBoxService.GetAllWithDetailsAsync();
                return Ok(new ResponseModel<List<MangaBoxGetAllDto>>
                {
                    Data = response,
                    Error = null,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<MangaBoxGetAllDto>
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 400
                });
            }
        }

        [HttpGet("get-mystery-box-detail/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _mangaBoxService.GetByIdWithDetailsAsync(id);
            if (data == null)
            {
                return NotFound(new ResponseModel<string>
                {
                    Success = false,
                    Data = null,
                    Error = "MangaBox not found",
                    ErrorCode = 404
                });
            }

            return Ok(new ResponseModel<MangaBoxDetailDto>
            {
                Success = true,
                Data = data,
                Error = null,
                ErrorCode = 0
            });
        }

    }
}
