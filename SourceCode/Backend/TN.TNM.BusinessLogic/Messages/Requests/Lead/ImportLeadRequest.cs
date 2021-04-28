using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class ImportLeadRequest : BaseRequest<ImportLeadParameter>
    {
        public List<IFormFile> FileList { get; set; }

        public override ImportLeadParameter ToParameter()
        {
            return new ImportLeadParameter()
            {
                UserId = this.UserId,
                FileList = this.FileList
            };
        }
    }
}
