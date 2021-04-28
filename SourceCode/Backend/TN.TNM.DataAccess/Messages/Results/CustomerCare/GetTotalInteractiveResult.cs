using System.Collections.Generic;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.DataAccess.Messages.Results.CustomerCare
{
    public class GetTotalInteractiveResult : BaseResult
    {
        public List<GetTotalInteractiveEntityModel> ListCate { get; set; }
        public int TotalCare { get; set; }
    }
}
