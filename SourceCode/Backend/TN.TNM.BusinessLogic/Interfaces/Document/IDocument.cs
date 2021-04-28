using TN.TNM.BusinessLogic.Messages.Requests.Document;
using TN.TNM.BusinessLogic.Messages.Responses.Document;

namespace TN.TNM.BusinessLogic.Interfaces.Document
{
    public interface IDocument
    {
        DownloadDocumentByIdResponse DownloadDocumentById(DownloadDocumentByIdRequest request);
    }
}
