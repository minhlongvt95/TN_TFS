using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.BillSale;
using TN.TNM.DataAccess.Messages.Parameters.BillSale;
using TN.TNM.DataAccess.Models.BillSale;

namespace TN.TNM.BusinessLogic.Messages.Requests.BillSale
{
    public class AddOrEditBillSaleRequest : BaseRequest<AddOrEditBillSaleParameter>
    {
        public bool? IsCreate { get; set; }
        public BillSaleModel BillSale { get; set; }

        public override AddOrEditBillSaleParameter ToParameter()
        {
            var parameter = new AddOrEditBillSaleParameter()
            {
                BillSale = BillSale == null ? null : BillSale.ToEntityModel(),
                IsCreate = IsCreate,
                UserId=UserId
            };

            if(BillSale != null)
            {
                parameter.BillSale.ListBillSaleDetail = new List<BillSaleDetailEntityModel>();
                parameter.BillSale.ListCost = new List<BillSaleCostEntityModel>();
                BillSale.ListBillSaleDetail?.ForEach(item =>
                {
                    var temp = item.ToEntityModel();
                    temp.ListBillSaleDetailProductAttribute = new List<BillSaleDetailProductAttributeEntityModel>();
                    item.ListBillSaleDetailProductAttribute?.ForEach(attr =>
                    {
                        temp.ListBillSaleDetailProductAttribute.Add(attr.ToEntityModel());
                    });

                    parameter.BillSale.ListBillSaleDetail.Add(temp);
                });

                BillSale.ListCost?.ForEach(item =>
                {
                    parameter.BillSale.ListCost.Add(item.ToEntityModel());
                });
            }

            return parameter;
        }
    }
}
