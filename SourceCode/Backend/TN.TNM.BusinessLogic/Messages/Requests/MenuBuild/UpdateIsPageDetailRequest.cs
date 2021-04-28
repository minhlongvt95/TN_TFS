using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.MenuBuild;

namespace TN.TNM.BusinessLogic.Messages.Requests.MenuBuild
{
    public class UpdateIsPageDetailRequest : BaseRequest<UpdateIsPageDetailParameter>
    {
        public Guid MenuBuildId { get; set; }
        public bool IsPageDetail { get; set; }

        public override UpdateIsPageDetailParameter ToParameter()
        {
            return new UpdateIsPageDetailParameter()
            {
                UserId = UserId,
                MenuBuildId = MenuBuildId,
                IsPageDetail = IsPageDetail
            };
        }
    }
}
