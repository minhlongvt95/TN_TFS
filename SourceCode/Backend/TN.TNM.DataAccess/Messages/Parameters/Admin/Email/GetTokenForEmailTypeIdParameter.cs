using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Email
{
    public class GetTokenForEmailTypeIdParameter : BaseParameter
    {
        public Guid EmailTemplateTypeId { get; set; }
    }
}
