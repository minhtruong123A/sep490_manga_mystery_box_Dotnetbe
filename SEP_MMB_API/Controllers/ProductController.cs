using BusinessObjects.Dtos.Product;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace SEP_MMB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {
        [HttpGet("get-product/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var data = await productService.GetProductWithRarityByIdAsync(id);
                if (data == null)
                {
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Data = null,
                        Error = "Product on mystery box not found",
                        ErrorCode = 404
                    });
                }

                return Ok(new ResponseModel<ProductWithRarityDto>
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
        [Authorize]
        [HttpGet("get-product")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await productService.GetAllProductsWithRarityAsync();
                if (data == null)
                {
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Data = null,
                        Error = "Don't have any product here",
                        ErrorCode = 404
                    });
                }

                return Ok(new ResponseModel<List<ProductWithRarityForModeratorDto>>
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
        [Authorize]
        [HttpPost("create-new-product-for-system")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDto dto)
        {
            try
            {
                var data = await productService.CreateProductAsync(dto);
                if (data)
                {
                    return Ok(new ResponseModel<string>
                    {
                        Success = true,
                        Data = "Add product succesfully",
                        Error = null,
                        ErrorCode = 0
                    });
                }
                return NotFound(new ResponseModel<string>
                {
                    Success = false,
                    Data = null,
                    Error = "Fail to add product",
                    ErrorCode = 404
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

        [Authorize]
        [HttpPatch("block-unlock-product")]
        public async Task<IActionResult> ChangeStatusProduct(string id)
        {
            try
            {
                var data = await productService.ChangeStatusProduct(id);
                if (data == 0)
                {
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Data = null,
                        Error = "Product not found",
                        ErrorCode = 404
                    });
                }
                if (data == 1)
                {
                    return Ok(new ResponseModel<string>
                    {
                        Success = true,
                        Data = "Block/Unblock succesfully",
                        Error = null,
                        ErrorCode = 0
                    });
                }
                return NotFound(new ResponseModel<string>
                {
                    Success = false,
                    Data = null,
                    Error = "Faill to change status product",
                    ErrorCode = 404
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
