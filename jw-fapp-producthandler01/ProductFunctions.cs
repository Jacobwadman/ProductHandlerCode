using jw_fapp_producthandler01.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace jw_fapp_producthandler01
{
    public class ProductFunctions
    {
        private TableService _tableService;

        public ProductFunctions(ILogger<ProductFunctions> logger)
        {
            _tableService = new TableService();
        }

        //Fixa try catch på båda metoderna
        [Function("AddProduct")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "products")] HttpRequest req)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var product = JsonConvert.DeserializeObject<ProductDTO>(requestBody);

            await _tableService.AddProductAsync(product);

            return new OkResult();
        }

        [Function("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products")] HttpRequest req)
        {

            var products = await _tableService.GetAllProductsAsync();
            return new OkObjectResult(products);
        }
    }
}
