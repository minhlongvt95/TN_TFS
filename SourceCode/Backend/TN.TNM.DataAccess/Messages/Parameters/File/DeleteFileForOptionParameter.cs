using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.File
{
    public class DeleteFileForOptionParameter : BaseParameter
    {
        public string Option { get; set; }
        public string FileName { get; set; }
    }
}
