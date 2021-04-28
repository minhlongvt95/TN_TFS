using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class ImportListLeadRequest: BaseRequest<ImportListLeadParameter>
    {
        public List<DataAccess.Models.Lead.ImportLeadModel> ListImportLead { get; set; }

        public override ImportListLeadParameter ToParameter()
        {
            return new ImportListLeadParameter()
            {
                UserId = UserId,
                ListImportLead = ListImportLead
            };
        }
    }
}
