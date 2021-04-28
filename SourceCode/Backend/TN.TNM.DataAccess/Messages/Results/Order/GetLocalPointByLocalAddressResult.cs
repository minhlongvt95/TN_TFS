using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.LocalPoint;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetLocalPointByLocalAddressResult : BaseResult
    {
        public List<LocalPointEntityModel> ListLocalPoint { get; set; }
    }
}
