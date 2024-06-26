using FUD.Web.Models;
using FUD.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FUD.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> list = new();

            ResponseDto? response = await _productService.GetAllProductsAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(list);
        }

        public IActionResult Create()
        {
            return View(new ProductDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDto product)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return View(product);
            }
            ResponseDto? response = await _productService.CreateProductAsync(product);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product created.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = response?.Message;
                return View(product);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            ProductDto product = new();

            ResponseDto? response = await _productService.GetProductByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));

                return View(product);
            }
            else
            {
                TempData["error"] = response?.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductDto product)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return View(product);
            }
            ResponseDto? response = await _productService.UpdateProductAsync(product);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product updated.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = response?.Message;
                return View(product);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            ProductDto product = new();

            ResponseDto? response = await _productService.GetProductByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));

                return View(product);
            }
            else
            {
                TempData["error"] = response?.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ProductDto product)
        {
            if (!ModelState.IsValid)
            {
                if (product.Id != null && product.Id > 0)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    return View(product);
                }
            }
            ResponseDto? response = await _productService.DeleteProductAsync(product.Id);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product deleted.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = response?.Message;
                return View(product);
            }
        }
    }
}
