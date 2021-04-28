using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.BusinessLogic.Models.Order;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetCustomerOrderByIDResponse : BaseResponse
    {
        public CustomerOrderModel CustomerOrderObject { get; set; }
        public List<CustomerOrderDetailModel> ListCustomerOrderDetail { get; set; }
        public List<NoteModel> ListNote { get; set; }
    }
}
