using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Product
{
    public class ImportPriceProductRequest : BaseRequest<ImportPriceProductParamter>
    {
        public List<PriceProductEntityModel> ListPriceProduct { get; set; }
        public override ImportPriceProductParamter ToParameter()
        {
            return new ImportPriceProductParamter
            {
                ListPriceProduct = this.ListPriceProduct,
                UserId = this.UserId
            };
        }
    }
}
