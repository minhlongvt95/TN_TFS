using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class CreateCustomerParameter : BaseParameter
    {
        public Databases.Entities.Customer Customer { get; set; }
        public Databases.Entities.Contact Contact { get; set; }
        public  List<Databases.Entities.Contact> CustomerContactList { get; set; }
        public bool CreateByLead { get; set; }
        public bool IsFromLead { get; set; } // biến kiểm tra khách hàng định danh hay là khách hàng tiềm năng
    }
}
