using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class CreateUpdateWarehouseMasterdataResponse:BaseResponse
    {
        public DataAccess.Databases.Entities.Warehouse WarehouseEntityModel { get; set; }
        public List<DataAccess.Models.Employee.EmployeeEntityModel> ListEmployeeEntityModel { get; set; }
        public List<string> ListWarehouseCode { get; set; }
    }
}
