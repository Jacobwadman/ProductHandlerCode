using FunctionApp2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionApp2
{
    public class ProductFunctions
    {
        private readonly TableService _tableService;
        private readonly ILogger<ProductFunctions> _logger;

        public ProductFunctions(ILogger<ProductFunctions> logger, ILogger<TableService> tableServiceLogger)
        {
            _tableService = new TableService(tableServiceLogger); // Pass the ILogger<TableService> to the constructor
            _logger = logger;
        }

        [Function("AddProduct")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "products")] HttpRequest req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var product = JsonConvert.DeserializeObject<ProductDTO>(requestBody);

                await _tableService.AddProductAsync(product);

                return new OkResult();
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Invalid JSON format: {ex.Message}");
                return new BadRequestObjectResult("Invalid JSON format.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Function("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products")] HttpRequest req)
        {
            try
            {
                var products = await _tableService.GetAllProductsAsync();
                return new OkObjectResult(products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
