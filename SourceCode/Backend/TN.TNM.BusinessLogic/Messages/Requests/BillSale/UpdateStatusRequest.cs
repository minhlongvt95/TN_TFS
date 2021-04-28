using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.BillSale;

namespace TN.TNM.BusinessLogic.Messages.Requests.BillSale
{
    public class UpdateStatusRequest:BaseRequest<UpdateStatusParameter>
    {
        public Guid BillSaleId { get; set; }
        public Guid StatusId { get; set; }
        public string Note { get; set; }
        public override UpdateStatusParameter ToParameter()
        {
            return new UpdateStatusParameter()
            {
                BillSaleId = BillSaleId,
                Note = Note,
                StatusId =StatusId,
                UserId =UserId
            };
        }
    }
}
