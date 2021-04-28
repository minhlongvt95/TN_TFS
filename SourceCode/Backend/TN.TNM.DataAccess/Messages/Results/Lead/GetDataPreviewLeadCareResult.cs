using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetDataPreviewLeadCareResult : BaseResult
    {
        public DateTime EffecttiveFromDate { get; set; }
        public DateTime EffecttiveToDate { get; set; }
        public DateTime SendDate { get; set; }
        public string StatusName { get; set; }
        public string PreviewEmailName { get; set; }
        public string PreviewEmailTitle { get; set; }
        public string PreviewEmailContent { get; set; }
        public string PreviewSmsPhone { get; set; }
        public string PreviewSmsContent { get; set; }
    }
}
