using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Messages.Requests.Promotion;
using TN.TNM.BusinessLogic.Messages.Responses.Promotion;

namespace TN.TNM.BusinessLogic.Interfaces.Promotion
{
    public interface IPromotion
    {
        GetMasterDataCreatePromotionResponse GetMasterDataCreatePromotion(GetMasterDataCreatePromotionRequest request);
        CreatePromotionResponse CreatePromotion(CreatePromotionRequest request);
        GetMasterDataListPromotionResponse GetMasterDataListPromotion(GetMasterDataListPromotionRequest request);
        SearchListPromotionResponse SearchListPromotion(SearchListPromotionRequest request);
        GetMasterDataDetailPromotionResponse GetMasterDataDetailPromotion(GetMasterDataDetailPromotionRequest request);
        GetDetailPromotionResponse GetDetailPromotion(GetDetailPromotionRequest request);
        DeletePromotionResponse DeletePromotion(DeletePromotionRequest request);
        UpdatePromotionResponse UpdatePromotion(UpdatePromotionRequest request);
        CreateLinkForPromotionResponse CreateLinkForPromotion(CreateLinkForPromotionRequest request);
        DeleteLinkFromPromotionResponse DeleteLinkFromPromotion(DeleteLinkFromPromotionRequest request);
        CreateFileForPromotionResponse CreateFileForPromotion(CreateFileForPromotionRequest request);
        DeleteFileFromPromotionResponse DeleteFileFromPromotion(DeleteFileFromPromotionRequest request);
        CreateNoteForPromotionDetailResponse CreateNoteForPromotionDetail(CreateNoteForPromotionDetailRequest request);
        CheckPromotionByCustomerResponse CheckPromotionByCustomer(CheckPromotionByCustomerRequest request);
        GetApplyPromotionResponse GetApplyPromotion(GetApplyPromotionRequest request);
        CheckPromotionByAmountResponse CheckPromotionByAmount(CheckPromotionByAmountRequest request);
        CheckPromotionByProductResponse CheckPromotionByProduct(CheckPromotionByProductRequest request);
    }
}
