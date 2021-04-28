using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class FilterProductParameter:BaseParameter
    {
        public List<Guid> ListProductCategory { get; set; }
        public List<Guid> ListProductId { get; set; }
    }
}
