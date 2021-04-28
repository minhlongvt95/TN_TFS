using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.MenuBuild;

namespace TN.TNM.BusinessLogic.Messages.Requests.MenuBuild
{
    public class UpdateMenuBuildRequest : BaseRequest<UpdateMenuBuildParameter>
    {
        public DataAccess.Databases.Entities.MenuBuild MenuBuild { get; set; }

        public override UpdateMenuBuildParameter ToParameter()
        {
            return new UpdateMenuBuildParameter()
            {
                UserId = UserId,
                MenuBuild = MenuBuild
            };
        }
    }
}
