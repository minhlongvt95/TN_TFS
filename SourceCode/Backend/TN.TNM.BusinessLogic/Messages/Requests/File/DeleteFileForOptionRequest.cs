using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.File;

namespace TN.TNM.BusinessLogic.Messages.Requests.File
{
    public class DeleteFileForOptionRequest : BaseRequest<DeleteFileForOptionParameter>
    {
        public string Option { get; set; }
        public string FileName { get; set; }

        public override DeleteFileForOptionParameter ToParameter()
        {
            return new DeleteFileForOptionParameter()
            {
                Option = Option,
                FileName = FileName,
                UserId = UserId
            };
        }
    }
}
