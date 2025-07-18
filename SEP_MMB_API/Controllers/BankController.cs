using BusinessObjects;
using BusinessObjects.Dtos.Bank;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IBankService _bankService;
        public BankController(IAuthService authService, IBankService bankService)
        {
            _authService = authService;
            _bankService = bankService;
        }
        [Authorize]
        [HttpGet("get-all-bank")]
        public async Task<ActionResult<ResponseModel<List<Bank>>>> GetAllAsync()
        {
            try
            {
                
                var response = await _bankService.GetAllAsync();

                return Ok(new ResponseModel<List<Bank>>
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
        [HttpPost("system/add-bank")]
        public async Task<ActionResult<ResponseModel<string>>> CreateBankAsync([FromBody] List<BankCreateDto> dto)
        {
            try
            {

                var response = await _bankService.CreateAsync(dto);

                return Ok(new ResponseModel<string>
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
    }
}
