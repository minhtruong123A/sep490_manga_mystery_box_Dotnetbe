using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using BusinessObjects;
using BusinessObjects.Dtos;
using BusinessObjects.Dtos.Auth;
using BusinessObjects.Dtos.MangaBox;
using BusinessObjects.Dtos.ProductInMangaBox;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.User;
using BusinessObjects.Dtos.UserCollection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Net.payOS.Types;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [ApiExplorerSettings(GroupName = "test")]
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(
        IUserService userService,
        IAuthService authService,
        IMangaBoxService mailboxService,
        IMapper mapper,
        IUserCollectionService userCollectionService,
        ICommentService commentService,
        IPayOSService payOsService,
        IProductInMangaBoxService productInMangaBoxService,
        IUseDigitalWalletService useDigitalWalletService)
        : ControllerBase
    {
        [Tags("Server Test Fetch API Only")]
        [HttpGet("user")]
        public async Task<ActionResult<ResponseModel<List<UserInformationDto>>>> Get()
        {
            try
            {
                var usersDto = await userService.GetAllUsersAsync();
                return Ok(new ResponseModel<List<UserInformationDto>>()
                {
                    Data = usersDto,
                    Error = null,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<List<UserInformationDto>>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 400
                });
            }
        }

        [Tags("Server Test Fetch API Only")]
        [HttpDelete("delete-by-email")]
        public async Task<ActionResult<ResponseModel<string>>> DeleteByEmail(string email)
        {
            try
            {
                await userService.DeleteUserByEmailAsync(email);
                return Ok(new ResponseModel<string>
                {
                    Data = "User and associated email verification deleted successfully.",
                    Success = true,
                    Error = null
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

        [Tags("Server Test Fetch API Only")]
        [Authorize]
        [HttpGet("check-token-expiry")]
        public ActionResult<ResponseModel<BoolWrapper>> CheckTokenExpiry()
        {
            try
            {
                var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();

                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                    return Unauthorized(new ResponseModel<BoolWrapper>
                    {
                        Data = null,
                        Success = false,
                        Error = "Missing or invalid Authorization header",
                        ErrorCode = 401
                    });

                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();

                handler.ValidateToken(token, authService.GetValidationParameters(), out _);

                return Ok(new ResponseModel<BoolWrapper>
                {
                    Data = new BoolWrapper { Value = true },
                    Success = true,
                    Error = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new ResponseModel<BoolWrapper>
                {
                    Data = null,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }

        [Tags("Server Test Fetch API Only")]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<ResponseModel<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            try
            {
                var result = await authService.RefreshTokenAsync(dto.Token);

                return Ok(new ResponseModel<AuthResponseDto>
                {
                    Data = result,
                    Success = true,
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResponseModel<AuthResponseDto>
                {
                    Data = null,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 401
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<AuthResponseDto>
                {
                    Data = null,
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }

        [Tags("Server Test Fetch API Only")]
        [Authorize]
        [HttpDelete("delete-all-comment")]
        public async Task<ActionResult<ResponseModel<string>>> DeleteAllComment()
        {
            try
            {
                await commentService.DeleteAllCommentAsync();
                return Ok(new ResponseModel<string>
                {
                    Data = "all comment deleted successfully.",
                    Success = true,
                    Error = null
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

        [Tags("Server Test BACKEND Only")]
        [HttpPost("add-manga-box")]
        public async Task<ActionResult<ResponseModel<MangaBoxDto>>> AddMangaBox([FromQuery] MangaBoxDto mangaBoxDto)
        {
            try
            {
                var mangaBox = mapper.Map<MangaBox>(mangaBoxDto);
                mangaBox.Id = ObjectId.GenerateNewId().ToString();
                var addedMangabox = await mailboxService.AddAsync(mangaBox);
                return Ok(new ResponseModel<MangaBox>
                {
                    Data = addedMangabox,
                    Success = true,
                    Error = null
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

        [Tags("Server Test BACKEND Only")]
        [HttpPost("add-product-in-manga-box")]
        public async Task<ActionResult<ResponseModel<MangaBoxDto>>> AddProductInMangaBox([FromQuery] ProductInMangaBoxDto productInMangaBox)
        {
            try
            {
                var productInMangaBoxMap = mapper.Map<ProductInMangaBox>(productInMangaBox);
                //productInMangaBoxMap.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                await productInMangaBoxService.CreateProductInMangaBoxAsync(productInMangaBoxMap);
                return Ok(new ResponseModel<string>
                {
                    Data = "Added successfully",
                    Success = true,
                    Error = null
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

        [Tags("Server Test BACKEND Only")]
        [HttpPost("add-user-collection")]
        public async Task<ActionResult<ResponseModel<UserCollectionDto>>> AddUserCollection([FromQuery] UserCollectionDto dto)
        {
            try
            {
                var userCollection = mapper.Map<UserCollection>(dto);
                userCollection.Id = ObjectId.GenerateNewId().ToString();
                await userCollectionService.CreateUserCollectionAsync(userCollection);
                return Ok(new ResponseModel<string>
                {
                    Data = null,
                    Success = true,
                    Error = null
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

        [Tags("Server Test BACKEND Only")]
        [Authorize(Roles = "user")]
        [HttpGet("test-create")]
        public async Task<IActionResult> TestCreate()
        {
            var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);
            long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var items = new List<ItemData> { new ItemData("Gói test VIP", 1, 20000) };
            var result = await payOsService.CreatePaymentLinkAsync(
                orderCode,
                amount: 20000,
                description: "Test giao diện thanh toán",
                items: items,
                userId: account.Id
            );

            return Ok(new ResponseModel<object>
            {
                Data = new
                {
                    checkoutUrl = result.checkoutUrl,
                    qrCode = result.qrCode,
                    orderCode = result.orderCode
                },
                Success = true,
                Error = null
            });
        }

        [Tags("Server Test Fetch API Only")]
        [Authorize(Roles = "user")]
        [HttpPut("update-user-wallet")]
        public async Task<ActionResult<ResponseModel<string>>> UpdateWallet([FromQuery] int amount)
        {
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);
                var transactionCode = await useDigitalWalletService.UpdateWalletWithTransactionAsync(account.Id, amount);

                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Data = transactionCode,
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
