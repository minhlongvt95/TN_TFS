using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Note;
using TN.TNM.BusinessLogic.Messages.Requests.Note;
using TN.TNM.BusinessLogic.Messages.Responses.Note;

namespace TN.TNM.Api.Controllers
{
    public class NoteController : Controller
    {
        private readonly INote iNote;
        public NoteController(INote _iNote)
        {
            this.iNote = _iNote;
        }

        /// <summary>
        /// CreateNote
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/note/createNote")]
        [Authorize(Policy = "Member")]
        public CreateNoteResponse CreateNote(CreateNoteRequest request)
        {
            return this.iNote.CreateNote(request);
        }

        /// <summary>
        /// Disable/delete note
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/note/disableNote")]
        [Authorize(Policy = "Member")]
        public DisableNoteResponse DisableNote([FromBody]DisableNoteRequest request)
        {
            return this.iNote.DisableNote(request);
        }

        /// <summary>
        /// Create a note contains document
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/note/createNoteAndNoteDocument")]
        [Authorize(Policy = "Member")]
        public CreateNoteAndNoteDocumentResponse CreateNoteAndNoteDocument(
             CreateNoteAndNoteDocumentRequest request)
        {
            return this.iNote.CreateNoteAndNoteDocument(request);
        }

        /// <summary>
        /// Edit a note
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/note/editNoteById")]
        [Authorize(Policy = "Member")]
        public EditNoteByIdResponse EditNoteById(EditNoteByIdRequest request)
        {
            return this.iNote.EditNoteById(request);
        }

        /// <summary>
        /// Search note
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/note/searchNote")]
        [Authorize(Policy = "Member")]
        public SearchNoteResponse SearchNote([FromBody]SearchNoteRequest request)
        {
            return this.iNote.SearchNote(request);
        }

        //
        /// <summary>
        /// Search note
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/note/createNoteForCustomerDetail")]
        [Authorize(Policy = "Member")]
        public CreateNoteForCustomerDetailResponse CreateNoteForCustomerDetail([FromBody]CreateNoteForCustomerDetailRequest request)
        {
            return this.iNote.CreateNoteForCustomerDetail(request);
        }

        [HttpPost]
        [Route("api/note/createNoteForLeadDetail")]
        [Authorize(Policy = "Member")]
        public CreateNoteForLeadDetailResponse CreateNoteForLeadDetail([FromBody]CreateNoteForLeadDetailRequest request)
        {
            return this.iNote.CreateNoteForLeadDetail(request);
        }

        //
        [HttpPost]
        [Route("api/note/createNoteForOrderDetail")]
        [Authorize(Policy = "Member")]
        public CreateNoteForOrderDetailResponse CreateNoteForOrderDetail([FromBody]CreateNoteForOrderDetailRequest request)
        {
            return this.iNote.CreateNoteForOrderDetail(request);
        }

        [HttpPost]
        [Route("api/note/createNoteForQuoteDetail")]
        [Authorize(Policy = "Member")]
        public CreateNoteForQuoteDetailResponse CreateNoteForQuoteDetail([FromBody]CreateNoteForQuoteDetailRequest request)
        {
            return this.iNote.CreateNoteForQuoteDetail(request);
        }

        [HttpPost]
        [Route("api/note/createNoteForSaleBiddingDetail")]
        [Authorize(Policy = "Member")]
        public CreateNoteForSaleBiddingDetailResponse CreateNoteForSaleBiddingDetail([FromBody]CreateNoteForSaleBiddingDetailRequest request)
        {
            return this.iNote.CreateNoteForSaleBiddingDetail(request);
        }

        [HttpPost]
        [Route("api/note/createNoteForBillSaleDetail")]
        [Authorize(Policy = "Member")]
        public CreateNoteForBillSaleDetailResponse CreateNoteForBillSaleDetail([FromBody]CreateNoteForBillSaleDetailRequest request)
        {
            return this.iNote.CreateNoteForBillSaleDetail(request);
        }

        [HttpPost]
        [Route("api/note/createNoteForContract")]
        [Authorize(Policy = "Member")]
        public CreateNoteForContractResponse CreateNoteForContract([FromBody]CreateNoteForContractRequest request)
        {
            return this.iNote.CreateNoteForContract(request);
        }
        
        [HttpPost]
        [Route("api/note/createNoteForObject")]
        [Authorize(Policy = "Member")]
        public CreateNoteForObjectResponse CreateNoteForObject([FromForm]CreateNoteForObjectRequest request)
        {
            return this.iNote.CreateNoteForObject(request);
        }
    }
}
