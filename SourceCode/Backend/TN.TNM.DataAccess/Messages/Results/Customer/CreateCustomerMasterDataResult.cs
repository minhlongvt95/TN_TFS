using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.GeographicalArea;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class CreateCustomerMasterDataResult : BaseResult
    {
        public List<CategoryEntityModel> ListCustomerGroupEntity { get; set; }
        public List<CategoryEntityModel> ListEnterPriseTypeEntity { get; set; }
        public List<CategoryEntityModel> ListBusinessScaleEntity { get; set; }
        public List<CategoryEntityModel> ListPositionEntity { get; set; }
        public List<CategoryEntityModel> ListBusinessLocalEntity { get; set; }  
        public List<CategoryEntityModel> ListMainBusinessEntity { get; set; }
        public List<ProvinceEntityModel> ListProvinceEntityModel { get; set; }
        public List<DistrictEntityModel> ListDistrictEntityModel { get; set; }
        public List<WardEntityModel> ListWardEntityModel { get; set; }
        public List<EmployeeEntityModel> ListEmployeeEntityModel { get; set; }
        public List<string> ListCustomerCode { get; set; }
        public List<string> ListCustomerTax { get; set; }
        public List<GeographicalAreaEntityModel> ListArea { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
    }
}
