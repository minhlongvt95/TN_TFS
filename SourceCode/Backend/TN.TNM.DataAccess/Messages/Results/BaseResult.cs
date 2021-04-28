using System.Collections.Generic;
using TN.TNM.Common.CommonObject;

namespace TN.TNM.DataAccess.Messages.Results
{

    public class BaseResult
    {
        public bool Status { get; set; }

        public string Message { get; set; }

        public List<NoteObject> Notes { get; set; }

    }
}