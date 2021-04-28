using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Ward
{
    public class GetAllWardByDistrictIdParameter : BaseParameter
    {
        public Guid DistrictId { get; set; }
    }
}
