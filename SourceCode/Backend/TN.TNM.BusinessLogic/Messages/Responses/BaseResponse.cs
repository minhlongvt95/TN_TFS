using System.Collections.Generic;
using System.Net;
using TN.TNM.Common.CommonObject;

namespace TN.TNM.BusinessLogic.Messages.Responses
{
    public class BaseResponse
    {
        public BaseResponse() { }
        public string MessageCode { get; set; }

        public List<NoteObject> Notes { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}