using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoECommerceApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DemoECommerceApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[Controller]")]
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return new Product[]
            {
                new Product(1, "Oats", new Decimal(3.07)),
                new Product(2, "Toothpaste", new Decimal(10.89)),
                new Product(3, "Television", new Decimal(500.90))
            };
        }
    }
}