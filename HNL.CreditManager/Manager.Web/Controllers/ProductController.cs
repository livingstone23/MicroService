using Manager.Web.Models;
using Manager.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Manager.Web.Controllers
{
    public class ProductController : Controller
    {


        private readonly IProductService _productService;


        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto> list = new();

            //60.2 Se obtiene el token para enviarse en los llamados
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var response = await _productService.GetAllProductsAsync<ResponseDto>(accessToken);

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            return View(list);
        }


        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                //60.2 Se obtiene el token para enviarse en los llamados
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                var response = await _productService.CreateProductAsync<ResponseDto>(model, accessToken);
                //var response = await _productService.CreateProductAsync<ResponseDto>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View(model);
        }


        public async Task<IActionResult> ProductEdit(int productId)
        {
            //60.2 Se obtiene el token para enviarse en los llamados
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, accessToken);
            //var response = await _productService.GetProductByIdAsync<ResponseDto>(productId);
            if (response != null && response.IsSuccess)
            {
                ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductEdit(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                //60.2 Se obtiene el token para enviarse en los llamados
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                var response = await _productService.UpdateProductAsync<ResponseDto>(model, accessToken);
                //var response = await _productService.UpdateProductAsync<ResponseDto>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View(model);
        }


        //[Authorize(Roles = "Admin")]
        [Authorize]
        public async Task<IActionResult> ProductDelete(int productId)
        {
            //60.2 Se obtiene el token para enviarse en los llamados
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, accessToken);
            //var response = await _productService.GetProductByIdAsync<ResponseDto>(productId);
            if (response != null && response.IsSuccess)
            {
                ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }
        
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDelete(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                //60.2 Se obtiene el token para enviarse en los llamados
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                var response = await _productService.DeleteProductAsync<ResponseDto>(model.ProductId, accessToken);
                //var response = await _productService.DeleteProductAsync<ResponseDto>(model.ProductId);
                if (response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View(model);
        }




    }
}
