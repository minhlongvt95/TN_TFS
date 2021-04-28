using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Document
{
    public class DownloadDocumentByIdParameter:BaseParameter
    {
        public Guid DocumentId { get; set; }
    }
}
