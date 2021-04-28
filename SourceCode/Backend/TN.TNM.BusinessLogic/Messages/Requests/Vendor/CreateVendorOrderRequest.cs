using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class CreateVendorOrderRequest : BaseRequest<CreateVendorOrderParameter>
    {
        public VendorOrderModel VendorOrder { get; set; }
        public List<VendorOrderDetailModel> VendorOrderDetail { get; set; }
        public List<VendorOrderProcurementRequestMappingModel> ListVendorOrderProcurementRequestMapping { get; set; }
        public List<VendorOrderCostDetailModel> ListVendorOrderCostDetail { get; set; }

        public override CreateVendorOrderParameter ToParameter()
        {
            List<VendorOrderDetail> lst = new List<VendorOrderDetail>();
            VendorOrderDetail.ForEach(item =>
            {
                var vendorOrderDetailObject = new VendorOrderDetail();
                vendorOrderDetailObject = item.ToEntity();
                List<VendorOrderProductDetailProductAttributeValue> orderProductDetailProductAttributeValueList =
                    new List<VendorOrderProductDetailProductAttributeValue>();
                if (item.VendorOrderProductDetailProductAttributeValue != null)
                {
                    item.VendorOrderProductDetailProductAttributeValue.ForEach(itemX =>
                    {
                        orderProductDetailProductAttributeValueList.Add(itemX.ToEntity());
                    });
                    vendorOrderDetailObject.VendorOrderProductDetailProductAttributeValue =
                        orderProductDetailProductAttributeValueList;
                }

                lst.Add(vendorOrderDetailObject);
            });
            
            var _listVendorOrderProcurementRequestMapping = new List<VendorOrderProcurementRequestMappingEntityModel>();
            ListVendorOrderProcurementRequestMapping.ForEach(item =>
            {
                _listVendorOrderProcurementRequestMapping.Add(item.ToEntity());
            });
            
            var _listVendorOrderCostDetail = new List<VendorOrderCostDetailEntityModel>();
            ListVendorOrderCostDetail.ForEach(item =>
            {
                _listVendorOrderCostDetail.Add(item.ToEntity());
            });

            return new CreateVendorOrderParameter() {
                UserId = UserId,
                VendorOrder = VendorOrder.ToEntity(),
                VendorOrderDetail = lst,
                ListVendorOrderProcurementRequestMapping = _listVendorOrderProcurementRequestMapping,
                ListVendorOrderCostDetail = _listVendorOrderCostDetail
            };
        }
    }
}
