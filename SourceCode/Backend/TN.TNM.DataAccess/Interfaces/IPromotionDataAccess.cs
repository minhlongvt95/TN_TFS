using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;
using TN.TNM.DataAccess.Messages.Results.Promotion;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IPromotionDataAccess
    {
        GetMasterDataCreatePromotionResult
            GetMasterDataCreatePromotion(GetMasterDataCreatePromotionParameter parameter);

        CreatePromotionResult CreatePromotion(CreatePromotionParameter parameter);
        GetMasterDataListPromotionResult GetMasterDataListPromotion(GetMasterDataListPromotionParameter parameter);
        SearchListPromotionResult SearchListPromotion(SearchListPromotionParameter parameter);

        GetMasterDataDetailPromotionResult
            GetMasterDataDetailPromotion(GetMasterDataDetailPromotionParameter parameter);

        GetDetailPromotionResult GetDetailPromotion(GetDetailPromotionParameter parameter);
        DeletePromotionResult DeletePromotion(DeletePromotionParameter parameter);
        UpdatePromotionResult UpdatePromotion(UpdatePromotionParameter parameter);
        CreateLinkForPromotionResult CreateLinkForPromotion(CreateLinkForPromotionParameter parameter);
        DeleteLinkFromPromotionResult DeleteLinkFromPromotion(DeleteLinkFromPromotionParameter parameter);
        CreateFileForPromotionResult CreateFileForPromotion(CreateFileForPromotionParameter parameter);
        DeleteFileFromPromotionResult DeleteFileFromPromotion(DeleteFileFromPromotionParameter parameter);

        CreateNoteForPromotionDetailResult
            CreateNoteForPromotionDetail(CreateNoteForPromotionDetailParameter parameter);

        CheckPromotionByCustomerResult CheckPromotionByCustomer(CheckPromotionByCustomerParameter parameter);
        GetApplyPromotionResult GetApplyPromotion(GetApplyPromotionParameter parameter);
        CheckPromotionByAmountResult CheckPromotionByAmount(CheckPromotionByAmountParameter parameter);
        CheckPromotionByProductResult CheckPromotionByProduct(CheckPromotionByProductParameter parameter);
    }
}
