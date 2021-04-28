using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class ImportListLeadParameter: BaseParameter
    {
        public List<Models.Lead.ImportLeadModel> ListImportLead { get; set; }
    }
}
