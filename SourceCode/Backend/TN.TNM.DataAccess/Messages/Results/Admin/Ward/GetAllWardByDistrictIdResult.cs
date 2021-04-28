using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Ward
{
    public class GetAllWardByDistrictIdResult : BaseResult
    {
        public List<Databases.Entities.Ward> ListWard { get; set; }
    }
}
