using System;
using TN.TNM.DataAccess.Messages.Parameters.Document;

namespace TN.TNM.BusinessLogic.Messages.Requests.Document
{
    public class DownloadDocumentByIdRequest : BaseRequest<DownloadDocumentByIdParameter>
    {
        public Guid DocumentId { get; set; }

        public override DownloadDocumentByIdParameter ToParameter()
        {
            return new DownloadDocumentByIdParameter
            {
                DocumentId = DocumentId,
                UserId = this.UserId
            };
        }
    }
}
