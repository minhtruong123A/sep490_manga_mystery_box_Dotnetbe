﻿using BusinessObjects;
using BusinessObjects.Dtos.MangaBox;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.Schema_Response;
using DataAccessLayers.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;
using System.Security.Claims;

namespace SEP_MMB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SellProductController : ControllerBase
    {
        private readonly ISellProductService _sellProductService;
        private readonly IAuthService _authService;

        public SellProductController(ISellProductService sellProductService, IAuthService authService)
        {
            _sellProductService = sellProductService;
            _authService = authService;
        }

        [HttpGet("get-all-product-on-sale-of-user/{userId}")]
        public async Task<ActionResult<ResponseModel<List<SellProductGetAllDto>>>> GetAllProductOnSaleOfUser(string userId)
        {
            try
            {
                var result = await _sellProductService.GetAllProductOnSaleOfUserAsync(userId);
                return Ok(new ResponseModel<List<SellProductGetAllDto>>
                {
                    Data = result,
                    Error = null,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<SellProductGetAllDto>
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 400
                });
            }
        }

        [HttpGet("get-all-product-on-sale")]
        public async Task<ActionResult<ResponseModel<List<SellProductGetAllDto>>>> GetAllProductOnSale()
        {
            try
            {
                var result = await _sellProductService.GetAllProductOnSaleAsync();
                return Ok(new ResponseModel<List<SellProductGetAllDto>>
                {
                    Data = result,
                    Error = null,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<SellProductGetAllDto>
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 400
                });
            }
        }

        [HttpGet("get-product-on-sale/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var data = await _sellProductService.GetProductDetailByIdAsync(id);
                if (data == null)
                {
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Data = null,
                        Error = "Product on sale not found",
                        ErrorCode = 404
                    });
                }

                return Ok(new ResponseModel<SellProductDetailDto>
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
        [HttpPost("create-sell-product")]
        public async Task<ActionResult<ResponseModel<object>>> CreateSellProduct([FromBody] SellProductCreateDto dto)
        {
            var response = new ResponseModel<object>();
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                int exchangeCode = await _sellProductService.CreateSellProductAsync(dto, account.Id);

                response.Success = true;
                response.Data = new
                {
                    Message = "Sell Product created successfully.",
                    ExchangeCode = exchangeCode
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


        [Authorize(Roles = "user")]
        [HttpPost("buy-sell-product")]
        public async Task<ActionResult<ResponseModel<BuySellProductResponseDto>>> BuySellProduct([FromBody] BuySellProductRequestDto request)
        {
            try
            {
                var (account, _, _, _) = await _authService.GetUserWithTokens(HttpContext);
                var orderId = await _sellProductService.BuySellProductAsync(account.Id, request.SellProductId, request.Quantity);

                return Ok(new ResponseModel<BuySellProductResponseDto>
                {
                    Success = true,
                    Data = new BuySellProductResponseDto
                    {
                        Message = "Buy product on sale successfully!",
                        OrderId = orderId
                    },
                    Error = null,
                    ErrorCode = 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<BuySellProductResponseDto>
                {
                    Success = false,
                    Data = null,
                    Error = ex.Message,
                    ErrorCode = 400
                });
            }
        }

        [Authorize(Roles = "user")]
        [HttpPut("update-sell-product")]
        public async Task<ActionResult<ResponseModel<object>>> UpdateSellProduct([FromBody] UpdateSellProductDto dto)
        {
            var response = new ResponseModel<object>();
            try
            {

                bool exchangeCode = await _sellProductService.UpdateSellProductAsync(dto);

                response.Success = true;
                response.Data = new
                {
                    Message = "Sell Product updated successfully.",
                    ExchangeCode = exchangeCode
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

        [Authorize(Roles = "user")]
        [HttpPut("turn-on/off-sell-product")]
        public async Task<ActionResult<ResponseModel<object>>> ChangeStatusSellProduct(string sellProductId)
        {
            var response = new ResponseModel<object>();
            try
            {

                bool exchangeCode = await _sellProductService.ChangestatusSellProductAsync(sellProductId);

                response.Success = true;
                response.Data = new
                {
                    Message = "Sell Product updated successfully.",
                    ExchangeCode = exchangeCode
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
