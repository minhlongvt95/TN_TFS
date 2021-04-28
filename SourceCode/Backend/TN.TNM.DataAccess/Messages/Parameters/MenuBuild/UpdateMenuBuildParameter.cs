using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.MenuBuild
{
    public class UpdateMenuBuildParameter: BaseParameter
    {
        public Databases.Entities.MenuBuild MenuBuild { get; set; }
    }
}
