using BusinessObjects;
using BusinessObjects.Dtos.Collection;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.UserCollection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCollectionController(IUserCollectionService userCollectionService, IAuthService authService)
        : ControllerBase
    {
        [Authorize]
        [HttpGet("get-all-collection-of-profile")]
        public async Task<ActionResult<ResponseModel<List<UserCollectionGetAllDto>>>> GetAllCollectionOfProfile()
        {
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);

                var collectionDto = await userCollectionService.GetAllWithDetailsAsync(account.Id.ToString());

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

                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);

                var userCollection = new UserCollection
                {
                    UserId = account.Id.ToString(),
                    CollectionId = collectionId.ToString()
                };

                await userCollectionService.CreateUserCollectionAsync(userCollection);

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

                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);

                
                await userCollectionService.CreateUserCollectionByUserAsync(account.Id, dto);

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
