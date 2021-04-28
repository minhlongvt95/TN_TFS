using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Contract
{
    public class ContractDetailProductAttributeEntityModel
    {
        public Guid ContractDetailProductAttributeId { get; set; }
        public Guid? ContractDetailId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductAttributeCategoryId { get; set; }
        public Guid? ProductAttributeCategoryValueId { get; set; }
        public string NameProductAttributeCategoryValue { get; set; }
        public string NameProductAttributeCategory { get; set; }
    }
}
