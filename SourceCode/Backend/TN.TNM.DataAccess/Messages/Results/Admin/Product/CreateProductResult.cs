using System;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class CreateProductResult:BaseResult
    {
        public Guid ProductId { get; set; }
        public DataAccess.Models.Product.ProductEntityModel NewProduct { get; set; }
    }
}
