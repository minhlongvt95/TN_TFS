using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.MenuBuild;

namespace TN.TNM.BusinessLogic.Messages.Responses.MenuBuild
{
    public class GetMenuPageBySubMenuCodeResponse : BaseResponse
    {
        public List<MenuBuildEntityModel> ListMenuPage { get; set; }
    }
}
