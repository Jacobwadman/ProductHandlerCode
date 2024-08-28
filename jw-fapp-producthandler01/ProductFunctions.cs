using jw_fapp_producthandler01.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace jw_fapp_producthandler01
{
    public class ProductFunctions
    {
        private TableService _tableService;

        public ProductFunctions(ILoggerFactory loggerFactory)
        {
            _tableService = new TableService(loggerFactory);
        }

        [FunctionName("AddProduct")]
        public async Task<IActionResult> AddProduct(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "products")] HttpRequest req)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var product = JsonConvert.DeserializeObject<ProductDTO>(requestBody);

            await _tableService.AddProductAsync(product);

            return new OkResult();
        }

        [FunctionName("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products")] HttpRequest req)
        {

            var products = await _tableService.GetAllProductsAsync();
            return new OkObjectResult(products);
        }
    }
}
