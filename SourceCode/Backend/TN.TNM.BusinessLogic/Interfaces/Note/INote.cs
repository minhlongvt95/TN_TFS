using TN.TNM.BusinessLogic.Messages.Requests.Note;
using TN.TNM.BusinessLogic.Messages.Responses.Note;

namespace TN.TNM.BusinessLogic.Interfaces.Note
{
    public interface INote
    {
        /// <summary>
        /// CreateNote
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        CreateNoteResponse CreateNote(CreateNoteRequest request);
        /// <summary>
        /// DisableNote
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        DisableNoteResponse DisableNote(DisableNoteRequest request);

        CreateNoteAndNoteDocumentResponse CreateNoteAndNoteDocument(CreateNoteAndNoteDocumentRequest request);
        EditNoteByIdResponse EditNoteById(EditNoteByIdRequest request);
        SearchNoteResponse SearchNote(SearchNoteRequest request);
        CreateNoteForCustomerDetailResponse CreateNoteForCustomerDetail(CreateNoteForCustomerDetailRequest request);
        CreateNoteForLeadDetailResponse CreateNoteForLeadDetail(CreateNoteForLeadDetailRequest request);
        CreateNoteForOrderDetailResponse CreateNoteForOrderDetail(CreateNoteForOrderDetailRequest request);
        CreateNoteForQuoteDetailResponse CreateNoteForQuoteDetail(CreateNoteForQuoteDetailRequest request);
        CreateNoteForSaleBiddingDetailResponse CreateNoteForSaleBiddingDetail(CreateNoteForSaleBiddingDetailRequest request);
        CreateNoteForContractResponse CreateNoteForContract(CreateNoteForContractRequest request);
        CreateNoteForBillSaleDetailResponse CreateNoteForBillSaleDetail(CreateNoteForBillSaleDetailRequest request);
        CreateNoteForObjectResponse CreateNoteForObject(CreateNoteForObjectRequest request);
    }
}
