using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetDataSearchEmployeeRequestResult : BaseResult
    {
        public List<Category> ListTypeRequest { get; set; }
        public List<Category> ListStatus { get; set; }
    }
}
