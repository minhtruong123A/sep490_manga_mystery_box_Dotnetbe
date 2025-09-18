using BusinessObjects.Dtos.Report;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController(IReportService reportService, IAuthService authService) : ControllerBase
    {
        [HttpGet("get-all-report")]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var data = await reportService.GetAllReportAsync();
                if (data == null)
                {
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Data = null,
                        Error = "Don't have any report",
                        ErrorCode = 404
                    });
                }

                return Ok(new ResponseModel<List<ReportResponeDto>>
                {
                    Success = true,
                    Data = data,
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

        [Authorize(Roles = "user")]
        [HttpGet("get-all-report-of-user")]
        public async Task<IActionResult> GetAllReportOfUserAsync()
        {
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);
                var data = await reportService.GetAllReportOfUserAsync(account.Id);
                if (data == null)
                {
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Data = null,
                        Error = "Don't have any report be found",
                        ErrorCode = 404
                    });
                }

                return Ok(new ResponseModel<List<ReportResponeDto>>
                {
                    Success = true,
                    Data = data,
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

        [Authorize(Roles = "user")]
        [HttpGet("get-sales-report-of-user")]
        public async Task<ActionResult<ResponseModel<SalesReportDto>>> GetSalesReportOfUserAsync()
        {
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);
                var data = await reportService.GetSalesReportAsync(account.Id);
                if (data == null)
                {
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Data = null,
                        Error = "No report found for this user.",
                        ErrorCode = 404
                    });
                }

                return Ok(new ResponseModel<SalesReportDto>
                {
                    Success = true,
                    Data = data,
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

        [Authorize(Roles = "user")]
        [HttpPost("create-report")]
        public async Task<ActionResult<ResponseModel<object>>> CreateReport([FromBody]ReportCreateDto dto)
        {
            var response = new ResponseModel<object>();
            try
            {
                var (account, _, _, _) = await authService.GetUserWithTokens(HttpContext);
                bool status = await reportService.CreateReportAsync(dto, account.Id);

                response.Success = true;
                response.Data = new
                {
                    Message = "Report has been sent.",
                    Status = status
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorCode = 400;
                return BadRequest(response);
            }
        }

        [Authorize]
        [HttpPatch("update-report")]
        public async Task<ActionResult<ResponseModel<string>>> UpdateStatus(string reportId)
        {
            var response = new ResponseModel<object>();
            try
            {
                bool status = await reportService.UpdateStatus(reportId);

                response.Success = true;
                response.Data = new
                {
                    Message = "Report has been processed.",
                    Status = status
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;
                response.ErrorCode = 400;
                return BadRequest(response);
            }
        }
    }
}
