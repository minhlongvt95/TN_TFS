using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.LocalPoint;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetLocalPointByLocalAddressResponse : BaseResponse
    {
        public List<LocalPointEntityModel> ListLocalPoint { get; set; }
    }
}
