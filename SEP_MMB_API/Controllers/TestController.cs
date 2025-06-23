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
using Net.payOS.Types;
using Services.Interface;
using Services.Service;
using System.IdentityModel.Tokens.Jwt;

namespace SEP_MMB_API.Controllers
{
    [ApiExplorerSettings(GroupName = "test")]
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IMangaBoxService _mangaBoxService;
        private readonly IUserCollectionService _userCollectionService;
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;
        private readonly IPayOSService _payOSService;
        private readonly IProductInMangaBoxService _productInMangaBoxService;

        public TestController(IUserService userService, IAuthService authService, IMangaBoxService mailboxService, IMapper mapper, IUserCollectionService userCollectionService, ICommentService commentService, IPayOSService payOSService, IProductInMangaBoxService productInMangaBoxService)
        {
            _userService = userService;
            _authService = authService;
            _mangaBoxService = mailboxService;
            _userCollectionService = userCollectionService;
            _commentService = commentService;
            _mapper = mapper;
            _payOSService = payOSService;
            _productInMangaBoxService = productInMangaBoxService;
        }

        [Tags("Server Test Fetch API Only")]
        [HttpGet("user")]
        public async Task<ActionResult<ResponseModel<List<UserInformationDto>>>> Get()
        {
            try
            {
                var usersDto = await _userService.GetAllUsersAsync();
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
                await _userService.DeleteUserByEmailAsync(email);
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
                
                handler.ValidateToken(token, _authService.GetValidationParameters(), out _);

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
                var result = await _authService.RefreshTokenAsync(dto.Token);

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
                await _commentService.DeleteAllCommentAsync();
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
                var mangaBox = _mapper.Map<MangaBox>(mangaBoxDto);
                mangaBox.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                var addedMangabox = await _mangaBoxService.AddAsync(mangaBox);
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
                var productInMangaBoxMap = _mapper.Map<ProductInMangaBox>(productInMangaBox);
                //productInMangaBoxMap.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                await _productInMangaBoxService.CreateProductInMangaBoxAsync(productInMangaBoxMap);
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
                var userCollection = _mapper.Map<UserCollection>(dto);
                userCollection.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                await _userCollectionService.CreateUserCollectionAsync(userCollection);
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
            var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
            long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var items = new List<ItemData> { new ItemData("Gói test VIP", 1, 20000) };
            var result = await _payOSService.CreatePaymentLinkAsync(
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
    }
}
