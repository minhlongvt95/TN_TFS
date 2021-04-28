using TN.TNM.DataAccess.Messages.Parameters.Note;
using TN.TNM.DataAccess.Messages.Results.Note;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface INoteDataAccess
    {
        /// <summary>
        /// CreateNote
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        CreateNoteResult CreateNote(CreateNoteParameter parameter);

        /// <summary>
        /// DisableNote
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        DisableNoteResult DisableNote(DisableNoteParameter parameter);

        CreateNoteAndNoteDocumentResult CreateNoteAndNoteDocument(CreateNoteAndNoteDocumentParameter parameter);
        EditNoteByIdResult EditNoteById(EditNoteByIdParameter parameter);
        SearchNoteResult SearchNote(SearchNoteParameter parameter);
        CreateNoteForCustomerDetailResult CreateNoteForCustomerDetail(CreateNoteForCustomerDetailParameter parameter);
        CreateNoteForLeadDetailResult CreateNoteForLeadDetail(CreateNoteForLeadDetailParameter parameter);
        CreateNoteForOrderDetailResult CreateNoteForOrderDetail(CreateNoteForOrderDetailParameter parameter);
        CreateNoteForQuoteDetailResult CreateNoteForQuoteDetail(CreateNoteForQuoteDetailParameter parameter);
        CreateNoteForSaleBiddingDetailResult CreateNoteForSaleBiddingDetail(CreateNoteForSaleBiddingDetailParameter parameter);
        CreateNoteForContractResult CreateNoteForContract(CreateNoteForContractParameter parameter);
        CreateNoteForBillSaleDetailResult CreateNoteForBillSaleDetail(CreateNoteForBillSaleDetailParameter parameter);
        CreateNoteForObjectResult CreateNoteForObject(CreateNoteForObjectParameter parameter);
    }
}
