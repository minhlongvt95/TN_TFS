using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.BillSale
{
    public class GetMasterDataBillSaleCreateEditParameter:BaseParameter
    {
        public bool IsCreate { get; set; }
        public Guid? ObjectId { get; set; }
    }
}
