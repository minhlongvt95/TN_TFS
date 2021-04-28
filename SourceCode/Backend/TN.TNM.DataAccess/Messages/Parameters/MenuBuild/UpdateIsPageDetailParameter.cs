using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.MenuBuild
{
    public class UpdateIsPageDetailParameter : BaseParameter
    {
        public Guid MenuBuildId { get; set; }
        public bool IsPageDetail { get; set; }
    }
}
