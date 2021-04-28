using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class ImportLeadDetailResult : BaseResult
    {
        public List<Models.Address.ProvinceEntityModel> ListProvince { get; set; }
        public List<Models.Address.DistrictEntityModel> ListDistrict { get; set; }
        public List<Models.Address.WardEntityModel> ListWard { get; set; }
        public List<Models.CategoryEntityModel> ListGender { get; set; }
        public List<Models.CategoryEntityModel> ListInterestedGroup { get; set; }
        public List<Models.CategoryEntityModel> ListPotential { get; set; }
        public List<Models.CategoryEntityModel> ListPaymentMethod { get; set; }
        public List<string> ListEmailLead { get; set; }
        public List<string> ListEmailCustomer { get; set; }
        public List<string> ListPhoneLead { get; set; }
        public List<string> ListPhoneCustomer { get; set; }
    }
}
