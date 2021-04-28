using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class SaveLeadCareFeedBackParameter : BaseParameter
    {
        public SaveLeadCareFeedBackModel LeadCareFeedBack { get; set; }
    }
}
