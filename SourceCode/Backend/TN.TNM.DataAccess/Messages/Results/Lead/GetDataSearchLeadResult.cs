using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.GeographicalArea;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetDataSearchLeadResult : BaseResult
    {
        public List<Models.CategoryEntityModel> ListPotential { get; set; }
        public List<Models.CategoryEntityModel> ListStatus { get; set; }
        public List<Models.CategoryEntityModel> ListInterestedGroup { get; set; }
        public List<Models.CategoryEntityModel> ListLeadType { get; set; }
        public List<Models.Employee.EmployeeEntityModel> ListPersonalInchange { get; set; }
        public List<Models.CategoryEntityModel> ListCusGroup { get; set; }
        public List<Models.CategoryEntityModel> ListSource { get; set; }
        public List<GeographicalAreaEntityModel> ListArea { get; set; }
    }
}
