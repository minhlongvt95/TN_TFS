using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TN.TNM.BusinessLogic.Interfaces.Promotion;
using TN.TNM.BusinessLogic.Messages.Requests.Promotion;
using TN.TNM.BusinessLogic.Messages.Responses.Promotion;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Promotion
{
    public class PromotionFactory : BaseFactory, IPromotion
    {
        private IPromotionDataAccess iPromotionDataAccess;

        public PromotionFactory(IPromotionDataAccess _iPromotionDataAccess)
        {
            this.iPromotionDataAccess = _iPromotionDataAccess;
        }

        public GetMasterDataCreatePromotionResponse GetMasterDataCreatePromotion(GetMasterDataCreatePromotionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iPromotionDataAccess.GetMasterDataCreatePromotion(parameter);
                var response = new GetMasterDataCreatePromotionResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListProduct = result.ListProduct,
                    ListCustomerGroup = result.ListCustomerGroup
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataCreatePromotionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreatePromotionResponse CreatePromotion(CreatePromotionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iPromotionDataAccess.CreatePromotion(parameter);
                var response = new CreatePromotionResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    PromotionId = result.PromotionId
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreatePromotionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataListPromotionResponse GetMasterDataListPromotion(GetMasterDataListPromotionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iPromotionDataAccess.GetMasterDataListPromotion(parameter);
                var response = new GetMasterDataListPromotionResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataListPromotionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SearchListPromotionResponse SearchListPromotion(SearchListPromotionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iPromotionDataAccess.SearchListPromotion(parameter);
                var response = new SearchListPromotionResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListPromotion = result.ListPromotion
                };

                return response;
            }
            catch (Exception e)
            {
                return new SearchListPromotionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataDetailPromotionResponse GetMasterDataDetailPromotion(GetMasterDataDetailPromotionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iPromotionDataAccess.GetMasterDataDetailPromotion(parameter);
                var response = new GetMasterDataDetailPromotionResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListProduct = result.ListProduct,
                    ListCustomerGroup = result.ListCustomerGroup
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataDetailPromotionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDetailPromotionResponse GetDetailPromotion(GetDetailPromotionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iPromotionDataAccess.GetDetailPromotion(parameter);
                var response = new GetDetailPromotionResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    Promotion = result.Promotion,
                    ListPromotionMapping = result.ListPromotionMapping,
                    ListLinkAndFile = result.ListLinkAndFile,
                    NoteHistory = result.NoteHistory
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetDetailPromotionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeletePromotionResponse DeletePromotion(DeletePromotionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iPromotionDataAccess.DeletePromotion(parameter);
                var response = new DeletePromotionResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new DeletePromotionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdatePromotionResponse UpdatePromotion(UpdatePromotionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iPromotionDataAccess.UpdatePromotion(parameter);
                var response = new UpdatePromotionResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdatePromotionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateLinkForPromotionResponse CreateLinkForPromotion(CreateLinkForPromotionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iPromotionDataAccess.CreateLinkForPromotion(parameter);
                var response = new CreateLinkForPromotionResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListLinkAndFile = result.ListLinkAndFile
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateLinkForPromotionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            };
        }

        public DeleteLinkFromPromotionResponse DeleteLinkFromPromotion(DeleteLinkFromPromotionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iPromotionDataAccess.DeleteLinkFromPromotion(parameter);
                var response = new DeleteLinkFromPromotionResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new DeleteLinkFromPromotionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateFileForPromotionResponse CreateFileForPromotion(CreateFileForPromotionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iPromotionDataAccess.CreateFileForPromotion(parameter);
                var response = new CreateFileForPromotionResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListLinkAndFile = result.ListLinkAndFile
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateFileForPromotionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteFileFromPromotionResponse DeleteFileFromPromotion(DeleteFileFromPromotionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iPromotionDataAccess.DeleteFileFromPromotion(parameter);
                var response = new DeleteFileFromPromotionResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new DeleteFileFromPromotionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateNoteForPromotionDetailResponse CreateNoteForPromotionDetail(CreateNoteForPromotionDetailRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iPromotionDataAccess.CreateNoteForPromotionDetail(parameter);
                var response = new CreateNoteForPromotionDetailResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    NoteHistory = result.NoteHistory
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateNoteForPromotionDetailResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CheckPromotionByCustomerResponse CheckPromotionByCustomer(CheckPromotionByCustomerRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iPromotionDataAccess.CheckPromotionByCustomer(parameter);
                var response = new CheckPromotionByCustomerResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    IsPromotionCustomer = result.IsPromotionCustomer
                };

                return response;
            }
            catch (Exception e)
            {
                return new CheckPromotionByCustomerResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetApplyPromotionResponse GetApplyPromotion(GetApplyPromotionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iPromotionDataAccess.GetApplyPromotion(parameter);
                var response = new GetApplyPromotionResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    ListPromotionApply = result.ListPromotionApply
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetApplyPromotionResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CheckPromotionByAmountResponse CheckPromotionByAmount(CheckPromotionByAmountRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iPromotionDataAccess.CheckPromotionByAmount(parameter);
                var response = new CheckPromotionByAmountResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    IsPromotionAmount = result.IsPromotionAmount
                };

                return response;
            }
            catch (Exception e)
            {
                return new CheckPromotionByAmountResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CheckPromotionByProductResponse CheckPromotionByProduct(CheckPromotionByProductRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iPromotionDataAccess.CheckPromotionByProduct(parameter);
                var response = new CheckPromotionByProductResponse()
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.FailedDependency,
                    MessageCode = result.Message,
                    IsPromotionProduct = result.IsPromotionProduct
                };

                return response;
            }
            catch (Exception e)
            {
                return new CheckPromotionByProductResponse()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
    }
}
