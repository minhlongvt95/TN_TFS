using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class ImportLeadDetailResponse: BaseResponse
    {
        public List<Models.Admin.ProvinceModel> ListProvince { get; set; }
        public List<Models.Admin.DistrictModel> ListDistrict { get; set; }
        public List<Models.Admin.WardModel> ListWard { get; set; }
        public List<Models.Category.CategoryModel> ListGender { get; set; }
        public List<Models.Category.CategoryModel> ListInterestedGroup { get; set; }
        public List<Models.Category.CategoryModel> ListPotential { get; set; }
        public List<Models.Category.CategoryModel> ListPaymentMethod { get; set; }
        public List<string> ListEmailLead { get; set; }
        public List<string> ListEmailCustomer { get; set; }
        public List<string> ListPhoneLead { get; set; }
        public List<string> ListPhoneCustomer { get; set; }
    }
}
