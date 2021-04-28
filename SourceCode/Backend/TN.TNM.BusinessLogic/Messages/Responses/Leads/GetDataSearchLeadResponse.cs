using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.GeographicalArea;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetDataSearchLeadResponse: BaseResponse
    {
        public List<Models.Category.CategoryModel> ListPotential { get; set; }
        public List<Models.Category.CategoryModel> ListStatus { get; set; }
        public List<Models.Category.CategoryModel> ListInterestedGroup { get; set; }
        public List<Models.Category.CategoryModel> ListLeadType { get; set; }
        public List<Models.Employee.EmployeeModel> ListPersonalInchange { get; set; }
        public List<CategoryEntityModel> ListCusGroup { get; set; }
        public List<CategoryEntityModel> ListSource { get; set; }
        public List<GeographicalAreaEntityModel> ListArea { get; set; }
    }
}
