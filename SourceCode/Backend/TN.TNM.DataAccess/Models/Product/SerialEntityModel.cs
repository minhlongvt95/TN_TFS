using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Product
{
    public class SerialEntityModel
    {
        public Guid? SerialId { get; set; }
        public string SerialCode { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? WarehouseId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
