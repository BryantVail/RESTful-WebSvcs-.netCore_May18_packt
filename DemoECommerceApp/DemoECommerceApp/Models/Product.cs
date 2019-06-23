using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoECommerceApp.Models
{
    public class Product
    {
        public Product(int id, string productName, decimal price )
        {
            this.Price = price;
            this.Id = id;
            this.ProductName = productName;
        }

        public decimal Price { get;  set; }
        public int Id { get; set;  }
        public string ProductName { get;  set;  }
    }
}
