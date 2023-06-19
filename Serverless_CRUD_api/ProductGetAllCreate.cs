using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Serverless_CRUD_api
{
    public class ProductGetAllCreate
    {

        private readonly AppDbContext _context;
        public ProductGetAllCreate(AppDbContext context)
        {
            _context = context;
        }

        [FunctionName("ProductGetAllCreate")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "products")] HttpRequest req)
        {
            if (req.Method == HttpMethods.Post)
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var product = JsonConvert.DeserializeObject<Product>(requestBody);
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return new CreatedResult("/products", product);
            }

            var products = await _context.Products.ToListAsync();
            return new OkObjectResult(products);
        }
    }
}
