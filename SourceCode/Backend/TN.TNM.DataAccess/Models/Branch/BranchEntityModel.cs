using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Branch
{
    public class BranchEntityModel
    {
        public Guid BranchId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
    }
}
