using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.DataAccess.Messages.Parameters.Customer;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class UpdateCustomerByIdRequest : BaseRequest<UpdateCustomerByIdParameter>
    {
        public CustomerEntityModel CustomerModel { get; set; }
        public ContactEntityModel ContactModel { get; set; }
        public override UpdateCustomerByIdParameter ToParameter()
        {
            return new UpdateCustomerByIdParameter()
            {
                UserId = UserId,
                CustomerModel = CustomerModel,
                ContactModel = ContactModel
            };
        }
    }
}
