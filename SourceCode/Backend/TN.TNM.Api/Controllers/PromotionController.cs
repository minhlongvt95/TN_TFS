using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Promotion;
using TN.TNM.BusinessLogic.Messages.Requests.Promotion;
using TN.TNM.BusinessLogic.Messages.Responses.Promotion;

namespace TN.TNM.Api.Controllers
{
    public class PromotionController : Controller
    {
        private readonly IPromotion iPromotion;
        public PromotionController(IPromotion _iPromotion)
        {
            this.iPromotion = _iPromotion;
        }

        [HttpPost]
        [Route("api/promotion/getMasterDataCreatePromotion")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreatePromotionResponse GetMasterDataCreatePromotion([FromBody]GetMasterDataCreatePromotionRequest request)
        {
            return iPromotion.GetMasterDataCreatePromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/createPromotion")]
        [Authorize(Policy = "Member")]
        public CreatePromotionResponse CreatePromotion([FromBody]CreatePromotionRequest request)
        {
            return iPromotion.CreatePromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/getMasterDataListPromotion")]
        [Authorize(Policy = "Member")]
        public GetMasterDataListPromotionResponse GetMasterDataListPromotion([FromBody]GetMasterDataListPromotionRequest request)
        {
            return iPromotion.GetMasterDataListPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/searchListPromotion")]
        [Authorize(Policy = "Member")]
        public SearchListPromotionResponse SearchListPromotion([FromBody]SearchListPromotionRequest request)
        {
            return iPromotion.SearchListPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/getMasterDataDetailPromotion")]
        [Authorize(Policy = "Member")]
        public GetMasterDataDetailPromotionResponse GetMasterDataDetailPromotion([FromBody]GetMasterDataDetailPromotionRequest request)
        {
            return iPromotion.GetMasterDataDetailPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/getDetailPromotion")]
        [Authorize(Policy = "Member")]
        public GetDetailPromotionResponse GetDetailPromotion([FromBody]GetDetailPromotionRequest request)
        {
            return iPromotion.GetDetailPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/deletePromotion")]
        [Authorize(Policy = "Member")]
        public DeletePromotionResponse DeletePromotion([FromBody]DeletePromotionRequest request)
        {
            return iPromotion.DeletePromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/updatePromotion")]
        [Authorize(Policy = "Member")]
        public UpdatePromotionResponse UpdatePromotion([FromBody]UpdatePromotionRequest request)
        {
            return iPromotion.UpdatePromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/createLinkForPromotion")]
        [Authorize(Policy = "Member")]
        public CreateLinkForPromotionResponse CreateLinkForPromotion([FromBody]CreateLinkForPromotionRequest request)
        {
            return iPromotion.CreateLinkForPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/deleteLinkFromPromotion")]
        [Authorize(Policy = "Member")]
        public DeleteLinkFromPromotionResponse DeleteLinkFromPromotion([FromBody]DeleteLinkFromPromotionRequest request)
        {
            return iPromotion.DeleteLinkFromPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/createFileForPromotion")]
        [Authorize(Policy = "Member")]
        public CreateFileForPromotionResponse CreateFileForPromotion([FromForm]CreateFileForPromotionRequest request)
        {
            return iPromotion.CreateFileForPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/deleteFileFromPromotion")]
        [Authorize(Policy = "Member")]
        public DeleteFileFromPromotionResponse DeleteFileFromPromotion([FromBody]DeleteFileFromPromotionRequest request)
        {
            return iPromotion.DeleteFileFromPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/createNoteForPromotionDetail")]
        [Authorize(Policy = "Member")]
        public CreateNoteForPromotionDetailResponse CreateNoteForPromotionDetail([FromBody]CreateNoteForPromotionDetailRequest request)
        {
            return iPromotion.CreateNoteForPromotionDetail(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/checkPromotionByCustomer")]
        [Authorize(Policy = "Member")]
        public CheckPromotionByCustomerResponse CheckPromotionByCustomer([FromBody]CheckPromotionByCustomerRequest request)
        {
            return iPromotion.CheckPromotionByCustomer(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/getApplyPromotion")]
        [Authorize(Policy = "Member")]
        public GetApplyPromotionResponse GetApplyPromotion([FromBody]GetApplyPromotionRequest request)
        {
            return iPromotion.GetApplyPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/checkPromotionByAmount")]
        [Authorize(Policy = "Member")]
        public CheckPromotionByAmountResponse CheckPromotionByAmount([FromBody]CheckPromotionByAmountRequest request)
        {
            return iPromotion.CheckPromotionByAmount(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/checkPromotionByProduct")]
        [Authorize(Policy = "Member")]
        public CheckPromotionByProductResponse CheckPromotionByProduct([FromBody]CheckPromotionByProductRequest request)
        {
            return iPromotion.CheckPromotionByProduct(request);
        }
    }
}
