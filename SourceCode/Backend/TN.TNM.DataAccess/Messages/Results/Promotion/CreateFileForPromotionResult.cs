using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.LinkOfDocument;

namespace TN.TNM.DataAccess.Messages.Results.Promotion
{
    public class CreateFileForPromotionResult : BaseResult
    {
        public List<LinkAndFileModel> ListLinkAndFile { get; set; }
    }
}
