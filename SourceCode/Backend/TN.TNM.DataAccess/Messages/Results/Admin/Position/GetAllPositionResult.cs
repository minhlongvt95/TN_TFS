using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Position
{
    public class GetAllPositionResult : BaseResult
    {
        public List<Databases.Entities.Position> ListPosition { get; set; }

    }
}
