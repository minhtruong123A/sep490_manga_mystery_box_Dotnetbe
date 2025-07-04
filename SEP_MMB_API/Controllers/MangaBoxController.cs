﻿using BusinessObjects.Dtos.MangaBox;
using BusinessObjects.Dtos.Schema_Response;
using BusinessObjects.Dtos.UserBox;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MangaBoxController : ControllerBase
    {
        private readonly IMangaBoxService _mangaBoxService;
        private readonly IAuthService _authService;

        public MangaBoxController(IMangaBoxService mangaBoxService, IAuthService authService)
        {
            _mangaBoxService = mangaBoxService;
            _authService = authService;
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
            try
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
        [HttpPost("buy-mystery-box")]
        public async Task<ActionResult<ResponseModel<BuyBoxResponseDto>>> BuyBox([FromBody] BuyBoxRequestDto request)
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var transactionCode = await _mangaBoxService.BuyBoxAsync(account.Id, request.MangaBoxId, request.Quantity);

                return Ok(new ResponseModel<BuyBoxResponseDto>
                {
                    Success = true,
                    Data = new BuyBoxResponseDto
                    {
                        Message = "Buy mystery box successfully!",
                        TransactionCode = transactionCode
                    },
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<BuyBoxResponseDto>
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
