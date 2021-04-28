using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Document;
using TN.TNM.DataAccess.Messages.Results.Document;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class DocumentDAO : BaseDAO, IDocumentDataAccess
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public DocumentDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment _hostingEnvironment)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            this.hostingEnvironment = _hostingEnvironment;
        }

        public DownloadDocumentByIdResult DownloadDocumentById(DownloadDocumentByIdParameter Parameter)
        {
            var document = context.Document.Where(w => w.DocumentId == Parameter.DocumentId).FirstOrDefault();
            if (document != null)
            {
                var dataByte= File.ReadAllBytes(document.DocumentUrl);
                return new DownloadDocumentByIdResult
                {
                    ExcelFile = dataByte,
                    NameFile = document.Name
                };
            }
            else
            {
                return new DownloadDocumentByIdResult
                {
                    ExcelFile = null,
                    NameFile = null
                };
            }
        }
    }
}
