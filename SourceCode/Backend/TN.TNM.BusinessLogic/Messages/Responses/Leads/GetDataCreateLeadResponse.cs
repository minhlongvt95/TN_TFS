using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.GeographicalArea;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetDataCreateLeadResponse: BaseResponse
    {
        public List<string> ListEmailLead { get; set; }   
        public List<string> ListPhoneLead { get; set; }
        public List<DataAccess.Models.Lead.CheckDuplicateLeadWithCustomerEntityModel> ListCustomerContact { get; set; }
        public List<Models.Category.CategoryModel> ListInterestedGroup { get; set; }
        public List<Models.Category.CategoryModel> ListGender { get; set; }
        public List<Models.Category.CategoryModel> ListPotential { get; set; }
        public List<Models.Category.CategoryModel> ListLeadGroup { get; set; }
        public List<Models.Category.CategoryModel> ListLeadType { get; set; }
        public List<DataAccess.Models.Employee.EmployeeEntityModel> ListPersonalInChange { get; set; }

        public List<DataAccess.Models.Address.ProvinceEntityModel> ListProvince { get; set; }
        public List<GeographicalAreaEntityModel> ListArea { get; set; }
        //NEW 
        public List<DataAccess.Models.CategoryEntityModel> ListBusinessType { get; set; }
        public List<DataAccess.Models.CategoryEntityModel> ListInvestFund { get; set; }
        public List<DataAccess.Models.CategoryEntityModel> ListProbability { get; set; }
        public List<DataAccess.Models.Lead.LeadReferenceCustomerModel> ListLeadReferenceCustomer { get; set; }

        // List Trạng thái chăm sóc
        public List<CategoryEntityModel> ListCareState { get; set; }
    }
}
