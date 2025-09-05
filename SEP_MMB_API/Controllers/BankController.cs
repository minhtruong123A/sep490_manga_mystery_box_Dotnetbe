using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController(IAuthService authService, IBankService bankService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IBankService _bankService = bankService;
        //[Authorize]
        //[HttpGet("get-all-bank")]
        //public async Task<ActionResult<ResponseModel<List<Bank>>>> GetAllAsync()
        //{
        //    try
        //    {
                
        //        var response = await _bankService.GetAllAsync();

        //        return Ok(new ResponseModel<List<Bank>>
        //        {
        //            Success = true,
        //            Data = response
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(400, new ResponseModel<object>
        //        {
        //            Success = false,
        //            Error = ex.Message,
        //            ErrorCode = 400
        //        });
        //    }
        //}

        //[Authorize]
        //[HttpPost("system/add-bank")]
        //public async Task<ActionResult<ResponseModel<string>>> CreateBankAsync([FromBody] List<BankCreateDto> dto)
        //{
        //    try
        //    {

        //        var response = await _bankService.CreateAsync(dto);

        //        return Ok(new ResponseModel<string>
        //        {
        //            Success = true,
        //            Data = response
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(400, new ResponseModel<object>
        //        {
        //            Success = false,
        //            Error = ex.Message,
        //            ErrorCode = 400
        //        });
        //    }
        //}
    }
}
