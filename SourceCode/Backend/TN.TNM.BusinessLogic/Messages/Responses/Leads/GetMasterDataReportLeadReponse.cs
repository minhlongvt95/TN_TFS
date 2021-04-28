using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.GeographicalArea;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetMasterDataReportLeadReponse : BaseResponse
    {
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<CategoryEntityModel> ListSource { get; set; }
        //public List<ProvinceEntityModel> ListProvince { get; set; }
        public List<GeographicalAreaEntityModel> ListArea { get; set; }
    }
}
