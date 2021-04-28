using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.LinkOfDocument;

namespace TN.TNM.BusinessLogic.Messages.Responses.Promotion
{
    public class CreateFileForPromotionResponse : BaseResponse
    {
        public List<LinkAndFileModel> ListLinkAndFile { get; set; }
    }
}
