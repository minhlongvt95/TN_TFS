using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.GeographicalArea;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetDataCreateLeadResult: BaseResult
    {
        public List<string> ListEmailLead { get; set; }
        public List<string> ListPhoneLead { get; set; }
        public List<Models.Lead.CheckDuplicateLeadWithCustomerEntityModel> ListCustomerContact { get; set; }
        public List<Models.Address.ProvinceEntityModel> ListProvince { get; set; }
        //public List<Models.Address.DistrictEntityModel> ListDistrict { get; set; }
        //public List<Models.Address.WardEntityModel> ListWard { get; set; }
        public List<Models.CategoryEntityModel> ListInterestedGroup { get; set; }
        public List<Models.CategoryEntityModel> ListGender { get; set; }
        //public List<Models.CategoryEntityModel> ListPaymentMethod { get; set; }
        public List<Models.CategoryEntityModel> ListPotential { get; set; }
        public List<Models.CategoryEntityModel> ListLeadType { get; set; }
        public List<Models.CategoryEntityModel> ListLeadGroup { get; set; }
        //public List<Models.Company.CompanyEntityModel> ListCompany { get; set; }
        public List<GeographicalAreaEntityModel> ListArea { get; set; }
        public List<Models.Employee.EmployeeEntityModel> ListPersonalInChange { get; set; }

        //NEW 
        public List<Models.CategoryEntityModel> ListBusinessType { get; set; }
        public List<Models.CategoryEntityModel> ListInvestFund { get; set; }
        public List<Models.CategoryEntityModel> ListProbability { get; set; }
        public List<Models.Lead.LeadReferenceCustomerModel> ListLeadReferenceCustomer { get; set; }

        // Trạng thái chăm sóc(Khách hàng tiềm năng)
        public List<CategoryEntityModel> ListCareState { get; set; }
    }
}
