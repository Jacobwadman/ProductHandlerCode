using Azure;
using Azure.Data.Tables;
using jw_fapp_producthandler01.Models;
using Microsoft.Extensions.Logging;

namespace jw_fapp_producthandler01
{
    public class TableService
    {
        private readonly TableClient _tableClient;
        private readonly ILogger<TableService> _logger;

        public TableService(ILogger<TableService> logger)
        {
            _tableClient = new TableClient(
                Environment.GetEnvironmentVariable("StorageAccountConnectionString"),
                Environment.GetEnvironmentVariable("ProductsTableName")
            );
            _logger = logger;
        }

        public async Task AddProductAsync(ProductDTO productDto)
        {
            var productEntity = new ProductTableEntity
            {
                PartitionKey = "Products",
                RowKey = Guid.NewGuid().ToString(),
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price
            };

            try
            {
                await _tableClient.AddEntityAsync(productEntity);
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Error adding product to table: {ex.Message}");
                throw new Exception("An error occurred while adding the product. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                throw new Exception("An unexpected error occurred. Please try again later.", ex);
            }
        }

        public async Task<List<ProductDTO>> GetAllProductsAsync()
        {
            var products = new List<ProductDTO>();

            try
            {
                await foreach (var entity in _tableClient.QueryAsync<ProductTableEntity>())
                {
                    var productDto = new ProductDTO
                    {
                        Name = entity.Name,
                        Description = entity.Description,
                        Price = entity.Price
                    };

                    products.Add(productDto);
                }
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Error retrieving products from table: {ex.Message}");
                throw new Exception("An error occurred while retrieving the products. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                throw new Exception("An unexpected error occurred. Please try again later.", ex);
            }

            return products;
        }
    }
}
