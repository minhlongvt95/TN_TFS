using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class UpdateQuoteParameter:BaseParameter
    {
        public Databases.Entities.Quote Quote { get; set; }
        public List<QuoteDetail> QuoteDetail { get; set; }
        public int TypeAccount { get; set; }
        public List<QuoteDocument> FileList { get; set; }
    }
}
