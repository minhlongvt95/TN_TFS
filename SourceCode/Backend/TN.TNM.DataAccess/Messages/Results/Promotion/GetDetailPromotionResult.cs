using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.LinkOfDocument;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Promotion;

namespace TN.TNM.DataAccess.Messages.Results.Promotion
{
    public class GetDetailPromotionResult : BaseResult
    {
        public PromotionEntityModel Promotion { get; set; }
        public List<PromotionMappingEntityModel> ListPromotionMapping { get; set; }
        public List<LinkAndFileModel> ListLinkAndFile { get; set; }
        public List<NoteEntityModel> NoteHistory { get; set; }
    }
}
