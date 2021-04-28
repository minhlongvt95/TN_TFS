using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class CreateUpdateWarehouseMasterdataResult: BaseResult
    {
        public Databases.Entities.Warehouse WarehouseEntityModel { get; set; }
        public List<DataAccess.Models.Employee.EmployeeEntityModel> ListEmployeeEntityModel { get; set; }
        public List<string> ListWarehouseCode { get; set; }
    }
}
