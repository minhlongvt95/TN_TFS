using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Product
{
    public class ImportProductRequest : BaseRequest<ImportProductParameter>
    {
        public List<DataAccess.Models.Product.ProductEntityModel> ListProduct { get; set; }
        public override ImportProductParameter ToParameter()
        {
            return new ImportProductParameter
            {
                UserId = UserId,
                ListProduct = ListProduct,
            };
        }
    }
}
