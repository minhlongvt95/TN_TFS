using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Product
{
    public class CreateProductResponse:BaseResponse
    {
        public Guid ProductId { get; set; }
        public DataAccess.Models.Product.ProductEntityModel NewProduct { get; set; }
    }
}
