﻿using Azure.Data.Tables;
using Azure.Identity;
using jw_fapp_producthandler01.Models;

namespace jw_fapp_producthandler01
{
    public class TableService
    {
        private readonly TableClient _tableClient;


        //private const string StorageAccountName = "jwtestsacc";
        //private const string ProductsTableName = "ProductsTable";

        //public TableService(ILoggerFactory loggerFactory)
        //{
        //    _logger = loggerFactory.CreateLogger<TableService>();


        //    _tableClient = new TableClient(
        //        new Uri($"https://{StorageAccountName}.table.core.windows.net/"),
        //        ProductsTableName,
        //        new DefaultAzureCredential()
        //    );
        //}
        public TableService()
        {



            _tableClient = new TableClient(
                new Uri($"https://{Environment.GetEnvironmentVariable("StorageAccountName")}.table.core.windows.net/"),
                Environment.GetEnvironmentVariable("ProductsTableName"),
                new DefaultAzureCredential()
            );
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
            catch (Exception ex)
            {
                throw;
            }
        }


        //SKippa loopen?
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
            catch (Exception ex)
            {
                throw;
            }

            return products;
        }
    }
}
