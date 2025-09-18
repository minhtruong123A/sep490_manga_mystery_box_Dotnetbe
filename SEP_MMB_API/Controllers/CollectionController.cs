using BusinessObjects;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionController(IAuthService authService, ICollectionService service) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [Authorize]
        [HttpGet("get-all-collection")]
        public async Task<ActionResult<ResponseModel<List<Collection>>>> GetAllAsync()
        {
            try
            {

                var response = await service.GetAllAsync();

                return Ok(new ResponseModel<List<Collection>>
                {
                    Success = true,
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ResponseModel<object>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }

        [Authorize]
        [HttpPost("crete-new-collection")]
        public async Task<ActionResult<ResponseModel<string>>> CreateNewCollectionAsync([FromBody]string topic)
        {
            try
            {

                var response = await service.CreateCollectionAsync(topic);
                if(response == 1)
                {
                    return Ok(new ResponseModel<string>
                    {
                        Success = true,
                        Data = "Create new collection succesfully"
                    });
                }
                return BadRequest(new ResponseModel<List<UserInformationDto>>()
                {
                    Data = null,
                    Error = "Collection existed",
                    Success = false,
                    ErrorCode = 400
                });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ResponseModel<object>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }
    }
}
