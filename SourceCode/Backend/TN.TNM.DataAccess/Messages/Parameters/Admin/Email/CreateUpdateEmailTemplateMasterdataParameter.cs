using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Email
{
    public class CreateUpdateEmailTemplateMasterdataParameter:BaseParameter
    {
        public Guid? EmailTemplateId { get; set; }
    }
}
