using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.LocalAddress;
using TN.TNM.DataAccess.Models.LocalPoint;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetMasterDataPayOrderServiceResponse : BaseResponse
    {
        public List<LocalAddressEntityModel> ListLocalAddress { get; set; }
        public List<LocalPointEntityModel> ListLocalPoint { get; set; }
        public decimal PointRate { get; set; }
        public decimal MoneyRate { get; set; }
    }
}
