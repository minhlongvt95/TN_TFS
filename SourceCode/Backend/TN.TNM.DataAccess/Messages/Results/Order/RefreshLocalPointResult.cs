using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.LocalAddress;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class RefreshLocalPointResult : BaseResult
    {
        public List<LocalAddressEntityModel> ListLocalAddress { get; set; }
    }
}
