using AutoMapper;
using BusinessObjects;
using BusinessObjects.Dtos.Exchange;
using BusinessObjects.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;

namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeController : ControllerBase
    {
        private readonly IExchangeService _service;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        public ExchangeController(IExchangeService service, IAuthService authService, IMapper mapper)
        {
            _service = service;
            _authService = authService;
            _mapper = mapper;
        }
   
        [Authorize]
        [HttpGet("with-products/by-receive")]
        public async Task<IActionResult> GetAllWithProducts()
        {
            var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
            var result = await _service.GetExchangesWithProductsByItemReciveIdAsync(account.Id);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("exchange-request-buyer")]
        public async Task<IActionResult> GetAllWithProductsOfBuyer()
        {
            var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
            if (account == null) return Unauthorized();

            var result = await _service.GetExchangesWithProductsByItemReciveIdAsync(account.Id);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("sender/create")]
        public async Task<IActionResult> CreateExchange([FromBody] CreateExchangeRequestDto dto)
        {
            if (dto.Products == null || !dto.Products.Any())
                return BadRequest("Product exchange list must not be empty.");

            var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
            if (account == null) return Unauthorized();

            var exchangeInfo = _mapper.Map<ExchangeInfo>(dto);
            exchangeInfo.BuyerId = account.Id;
            exchangeInfo.Status = (int)ExchangeStatus.Pending;
            exchangeInfo.Datetime = DateTime.UtcNow;

            var session = _mapper.Map<ExchangeSession>(dto.Session);
            session.Status = 0;

            var exchangeProducts = dto.Products.Select(p =>
            {
                var mapped = _mapper.Map<ExchangeProduct>(p);

                return mapped;
            }).ToList();

            var created = await _service.CreateExchangeAsync(exchangeInfo, exchangeProducts, session);

            return Ok(created);
        }

        [HttpPost("sender/accept/{exchangeId}")]
        public async Task<IActionResult> AcceptExchange(string exchangeId)
        {
            var success = await _service.AcceptExchangeAsync(exchangeId);
            if (!success) return BadRequest("Exchange not found or failed");
            return Ok("Exchange completed");
        }

        [Authorize]
        [HttpPost("recipient/cancel/{exchangeId}")]
        public async Task<IActionResult> CancelExchange(string exchangeId)
        {
            var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
            if (account == null) return Unauthorized();

            var success = await _service.CancelExchangeAsync(exchangeId, account.Id);
            return success ? Ok("Canceled") : BadRequest("Cancel failed");
        }

        [Authorize]
        [HttpPost("reject/{exchangeId}")]
        public async Task<IActionResult> RejectExchange(string exchangeId)
        {
            var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
            if (account == null) return Unauthorized();

            var success = await _service.RejectExchangeAsync(exchangeId, account.Id);
            return success ? Ok("Rejected") : BadRequest("Reject failed");
        }

    }

}
