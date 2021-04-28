using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetCustomerOrderByIDResult : BaseResult
    {
        public CustomerOrderEntityModel CustomerOrderObject { get; set; }
        public List<CustomerOrderDetailEntityModel> ListCustomerOrderDetail { get;set;}
        public List<NoteEntityModel> ListNote { get; set; }
    }
}
