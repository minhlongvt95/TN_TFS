using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Email
{
    public class CreateUpdateEmailTemplateParameter: BaseParameter
    {
        public DataAccess.Databases.Entities.EmailTemplate EmailTemplateEntityModel { get; set; }
        public List<string> ListEmailToCC { get; set; }
    }
}
