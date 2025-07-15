using Microsoft.AspNetCore.Mvc;
using BusinessObjects;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;
using BusinessObjects.Dtos.UserCollection;
using BusinessObjects.Dtos.Collection;
namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCollectionController : ControllerBase
    {
        private readonly IUserCollectionService _userCollectionService;
        private readonly IAuthService _authService;

        public UserCollectionController(IUserCollectionService userCollectionService, IAuthService authService)
        {
            _userCollectionService = userCollectionService;
            _authService = authService;
        }

        [Authorize]
        [HttpGet("get-all-collection-of-profile")]
        public async Task<ActionResult<ResponseModel<List<UserCollectionGetAllDto>>>> GetAllCollectionOfProfile()
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);

                var collectionDto = await _userCollectionService.GetAllWithDetailsAsync(account.Id.ToString());

                return Ok(new ResponseModel<List<UserCollectionGetAllDto>>
                {
                    Data = collectionDto,
                    Error = null,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<List<UserCollectionGetAllDto>>
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 400
                });
            }
        }

        [Authorize]
        [HttpPost("create-new-user-collection-for-system")]
        public async Task<ActionResult<ResponseModel<string>>> CreateUserCollection([FromHeader] string collectionId)
        {
            try
            {

                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);

                var userCollection = new UserCollection
                {
                    UserId = account.Id.ToString(),
                    CollectionId = collectionId.ToString()
                };

                await _userCollectionService.CreateUserCollectionAsync(userCollection);

                return Ok(new ResponseModel<string>
                {
                    Data = "UserCollection added successfully",
                    Success = true
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
        [HttpPost("create-new-user-collection-for-user")]
        public async Task<ActionResult<ResponseModel<string>>> CreateMyCollection([FromBody] CollectionCreateByUserDto dto)
        {
            try
            {

                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);

               
                await _userCollectionService.CreateUserCollectionByUserAsync(account.Id, dto);

                return Ok(new ResponseModel<string>
                {
                    Data = "UserCollection created successfully",
                    Success = true
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
