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
    public class ProductsGetByIdUpdateDelete
    {
        private readonly AppDbContext _context;
        public ProductsGetByIdUpdateDelete(AppDbContext context)
        {
            _context = context;
        }

        [FunctionName("ProductsGetByIdUpdateDelete")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "put", "delete", Route = "products/{id}")] HttpRequest req, int id)
        {
            if (req.Method == HttpMethods.Get)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product == null) return new NotFoundResult();

                return new OkObjectResult(product);
            }

            else if(req.Method == HttpMethods.Put)
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync(); 
                var product = JsonConvert.DeserializeObject<Product>(requestBody);
                product.Id = id;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return new OkObjectResult(product); 
            }

            else
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
                if(product == null) return new NotFoundResult();

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return new NoContentResult();
            }
        }
    }
}
