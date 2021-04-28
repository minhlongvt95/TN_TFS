using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Position
{
    public class GetAllPositionResponse : BaseResponse
    {
        public List<PositionModel> ListPosition { get; set; }
    }
}
