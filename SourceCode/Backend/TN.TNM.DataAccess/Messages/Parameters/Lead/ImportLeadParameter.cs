using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class ImportLeadParameter:BaseParameter
    {
        public List<IFormFile> FileList { get; set; }
    }
}
