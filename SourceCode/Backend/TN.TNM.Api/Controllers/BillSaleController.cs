using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.BillSale;
using TN.TNM.BusinessLogic.Messages.Requests.BillSale;
using TN.TNM.BusinessLogic.Messages.Responses.BillSale;

namespace TN.TNM.Api.Controllers
{
    public class BillSaleController : ControllerBase
    {
        private readonly IBillSale _iBillSale;
        public BillSaleController(IBillSale iBillSale)
        {
            this._iBillSale = iBillSale;
        }

        [HttpPost]
        [Route("api/billSale/getMasterDataBillSaleCreateEdit")]
        [Authorize(Policy = "Member")]
        public GetMasterDataBillSaleCreateEditResponse GetMasterDataBillSaleCreateEdit([FromBody]GetMasterDataBillSaleCreateEditRequest request)
        {
            return _iBillSale.GetMasterDataBillSaleCreateEdit(request);
        }

        [HttpPost]
        [Route("api/billSale/addOrEditBillSale")]
        [Authorize(Policy = "Member")]
        public AddOrEditBillSaleResponse AddOrEditBillSale([FromBody]AddOrEditBillSaleRequest request)
        {
            return _iBillSale.AddOrEditBillSale(request);
        }

        [HttpPost]
        [Route("api/billSale/searchBillSale")]
        [Authorize(Policy = "Member")]
        public SearchBillOfSaleResponse SearchBillOfSale([FromBody]SearchBillOfSaleRequest request)
        {
            return _iBillSale.SearchBillOfSale(request);
        }

        [HttpPost]
        [Route("api/billSale/getMasterDataSearchBillSale")]
        [Authorize(Policy = "Member")]
        public GetMasterBillOfSaleResponse GetMasterBillOfSale([FromBody]GetMasterBillOfSaleRequest request)
        {
            return _iBillSale.GetMasterBillOfSale(request);
        }

        [HttpPost]
        [Route("api/billSale/getOrderByOrderId")]
        [Authorize(Policy = "Member")]
        public GetOrderByOrderIdResponse GetOrderByOrderId([FromBody]GetOrderByOrderIdRequest request)
        {
            return _iBillSale.GetOrderByOrderId(request);
        }

        [HttpPost]
        [Route("api/billSale/updateStatus")]
        [Authorize(Policy = "Member")]
        public UpdateStatusResponse UpdateStatus([FromBody]UpdateStatusRequest request)
        {
            return _iBillSale.UpdateStatus(request);
        }

        [HttpPost]
        [Route("api/billSale/deleteBillSale")]
        [Authorize(Policy = "Member")]
        public DeleteBillSaleResponse DeleteBillSale([FromBody]DeleteBillSaleRequest request)
        {
            return _iBillSale.DeleteBillSale(request);
        }
    }
}