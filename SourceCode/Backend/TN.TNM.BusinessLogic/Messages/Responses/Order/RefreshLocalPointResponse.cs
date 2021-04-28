using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.LocalAddress;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class RefreshLocalPointResponse : BaseResponse
    {
        public List<LocalAddressEntityModel> ListLocalAddress { get; set; }
    }
}
