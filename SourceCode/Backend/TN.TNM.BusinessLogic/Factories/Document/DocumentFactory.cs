using System;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Document;
using TN.TNM.BusinessLogic.Messages.Requests.Document;
using TN.TNM.BusinessLogic.Messages.Responses.Document;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Document
{
    public class DocumentFactory : BaseFactory, IDocument
    {
        private IDocumentDataAccess iDocumentDataAccess;
        public DocumentFactory(IDocumentDataAccess _iDocumentDataAccess, ILogger<DocumentFactory> _logger)
        {
            iDocumentDataAccess = _iDocumentDataAccess;
            logger = _logger;
        }

        public DownloadDocumentByIdResponse DownloadDocumentById(DownloadDocumentByIdRequest request)
        {
            try
            {
                logger.LogInformation("Download Document ById");
                var parameter = request.ToParameter();
                var result = iDocumentDataAccess.DownloadDocumentById(parameter);
                var response = new DownloadDocumentByIdResponse()
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = result.Message,
                    ExcelFile=result.ExcelFile,
                    NameFile=result.NameFile
                };
                return response;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new DownloadDocumentByIdResponse()
                {
                    MessageCode = CommonMessage.Document.DOWNLOAD_FAIL,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
    }
}
