using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Email
{
    public class CreateUpdateEmailTemplateMasterdataResult : BaseResult
    {
        public List<DataAccess.Databases.Entities.Category> ListEmailType { get; set; }
        public List<DataAccess.Databases.Entities.EmailTemplateToken> ListToken { get; set; }
        public List<DataAccess.Databases.Entities.Category> ListEmailStatus {get;set;}
        public List<DataAccess.Databases.Entities.EmailTemplateToken> ListEmailTemplateToken { get; set; }
        public DataAccess.Databases.Entities.EmailTemplate EmailTemplateModel { get; set; }        
        public List<string> ListEmailToCC { get; set; }
    }
}
