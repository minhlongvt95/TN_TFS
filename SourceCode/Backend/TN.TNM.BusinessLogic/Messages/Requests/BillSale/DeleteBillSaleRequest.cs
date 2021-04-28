using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.BillSale;

namespace TN.TNM.BusinessLogic.Messages.Requests.BillSale
{
    public class DeleteBillSaleRequest:BaseRequest<DeleteBillSaleParameter>
    {
        public Guid BillSaleId { get; set; }
        public override DeleteBillSaleParameter ToParameter()
        {
            return new DeleteBillSaleParameter()
            {
                BillSaleId = BillSaleId,
                UserId =UserId
            };
        }
    }
}
