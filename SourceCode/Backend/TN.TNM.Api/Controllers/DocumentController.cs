using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Document;
using TN.TNM.BusinessLogic.Messages.Requests.Document;
using TN.TNM.BusinessLogic.Messages.Responses.Document;

namespace TN.TNM.Api.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IDocument iDocument;
        public DocumentController(IDocument _iDocument)
        {
            iDocument = _iDocument;
        }
        /// <summary>
        /// DownloadDocumentById
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/document/downloadDocumentById")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public DownloadDocumentByIdResponse DownloadDocumentById([FromBody]DownloadDocumentByIdRequest request)
        {
            return iDocument.DownloadDocumentById(request);
        }

    }
}