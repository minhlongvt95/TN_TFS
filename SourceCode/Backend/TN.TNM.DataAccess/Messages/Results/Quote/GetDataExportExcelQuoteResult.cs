using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Quote
{
    public class GetDataExportExcelQuoteResult : BaseResult
    {
        public InforExportExcelModel InforExportExcel { get; set; }
        public QuoteEntityModel Quote { get; set; }
        public List<QuoteDetailEntityModel> ListQuoteDetail { get; set; }
        public List<AdditionalInformationEntityModel> ListAdditionalInformation { get; set; }
    }

}
