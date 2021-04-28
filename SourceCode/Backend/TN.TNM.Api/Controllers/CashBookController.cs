using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.CashBook;
using TN.TNM.BusinessLogic.Messages.Requests.CashBook;
using TN.TNM.BusinessLogic.Messages.Responses.CashBook;

namespace TN.TNM.Api.Controllers
{
    public class CashBookController : Controller
    {
        private readonly ICashBook iCashBook;
        public CashBookController(ICashBook _iCashBook)
        {
            this.iCashBook = _iCashBook;
        }

        /// <summary>
        /// getSurplusCashBookPerMonth
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/cashbook/getSurplusCashBookPerMonth")]
        [Authorize(Policy = "Member")]
        public GetSurplusCashBookPerMonthResponse GetSurplusCashBookPerMonth([FromBody]GetSurplusCashBookPerMonthRequest request)
        {
            return iCashBook.GetSurplusCashBookPerMonth(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/cashbook/getDataSearchCashBook")]
        [Authorize(Policy = "Member")]
        public GetDataSearchCashBookResponse GetDataSearchCashBook([FromBody]GetDataSearchCashBookRequest request)
        {
            return iCashBook.GetDataSearchCashBook(request);
        }
    }
}