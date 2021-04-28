using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.BankBook;
using TN.TNM.BusinessLogic.Messages.Requests.BankBook;
using TN.TNM.BusinessLogic.Messages.Responses.BankBook;

namespace TN.TNM.Api.Controllers
{
    public class BankBookController : Controller
    {
        private readonly IBankBook iBankBook;
        public BankBookController(IBankBook _iBankBook)
        {
            this.iBankBook = _iBankBook;
        }

        /// <summary>
        /// Search Bank Book
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/bankbook/searchBankBook")]
        [Authorize(Policy = "Member")]
        public SearchBankBookResponse SearchBankBook([FromBody]SearchBankBookRequest request)
        {
            return iBankBook.SearchBankBook(request);
        }

        [HttpPost]
        [Route("api/bankbook/getMasterDataSearchBankBook")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchBankBookResponse GetMasterDataSearchBankBook([FromBody]GetMasterDataSearchBankBookRequest request)
        {
            return iBankBook.GetMasterDataSearchBankBook(request);
        }
    }
}