using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;


namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class ImportLeadDetailRequest: BaseRequest<ImportLeadDetailParameter>
    {
        public override ImportLeadDetailParameter ToParameter()
        {
            return new ImportLeadDetailParameter()
            {
                UserId = UserId
            };
        }
    }
}
