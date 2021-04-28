using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.GeographicalArea;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class CreateCustomerMasterDataResponse: BaseResponse
    {
        public List<CategoryModel> ListCustomerGroup { get; set; }
        public List<CategoryModel> ListEnterPriseType { get; set; }
        public List<CategoryModel> ListBusinessScale { get; set; }
        public List<CategoryModel> ListPosition { get; set; }
        public List<CategoryModel> ListBusinessLocal { get; set; }
        public List<CategoryModel> ListMainBusiness { get; set; }
        public List<ProvinceModel> ListProvinceModel { get; set; }
        public List<DistrictModel> ListDistrictModel { get; set; }
        public List<WardModel> ListWardModel { get; set; }
        public List<EmployeeModel> ListEmployeeModel { get; set; }
        public List<string> ListCustomerCode { get; set; }
        public List<string> ListCustomerTax { get; set; }
        public List<GeographicalAreaEntityModel> ListArea { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
    }
}
