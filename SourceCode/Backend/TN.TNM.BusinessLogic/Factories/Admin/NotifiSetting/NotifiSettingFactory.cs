using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Factories.Admin.Order_Status;
using TN.TNM.BusinessLogic.Interfaces.Admin.NotifiSetting;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.NotifiSetting;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.NotifiSetting;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Admin.NotifiSetting
{
    public class NotifiSettingFactory: BaseFactory, INotifiSetting
    {
        private INotifiSettingDataAccess iNotifiSettingDataAccess;

        public NotifiSettingFactory(INotifiSettingDataAccess _iNotifiSettingDataAccess)
        {
            this.iNotifiSettingDataAccess = _iNotifiSettingDataAccess;
        }

        public GetMasterDataNotifiSettingCreateResponse GetMasterDataNotifiSettingCreate(
            GetMasterDataNotifiSettingCreateRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNotifiSettingDataAccess.GetMasterDataNotifiSettingCreate(parameter);

                var response = new GetMasterDataNotifiSettingCreateResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListEmployee = result.ListEmployee,
                    ListNotifiAction = result.ListNotifiAction,
                    ListInforScreen = result.ListInforScreen,
                    ListScreen = result.ListScreen,
                    ListNotifiSettingToken = result.ListNotifiSettingToken
                };
                
                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataNotifiSettingCreateResponse
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public CreateNotifiSettingResponse CreateNotifiSetting(CreateNotifiSettingRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNotifiSettingDataAccess.CreateNotifiSetting(parameter);

                var response = new CreateNotifiSettingResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    NotifiSettingId = result.NotifiSettingId
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateNotifiSettingResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public GetMasterDataNotifiSettingDetailResponse GetMasterDataNotifiSettingDetail(
            GetMasterDataNotifiSettingDetailRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNotifiSettingDataAccess.GetMasterDataNotifiSettingDetail(parameter);

                var response = new GetMasterDataNotifiSettingDetailResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListEmployee = result.ListEmployee,
                    ListNotifiAction = result.ListNotifiAction,
                    ListInforScreen = result.ListInforScreen,
                    ListScreen = result.ListScreen,
                    NotifiSetting = result.NotifiSetting,
                    ListNotifiSpecial = result.ListNotifiSpecial,
                    ListNotifiSettingToken = result.ListNotifiSettingToken
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataNotifiSettingDetailResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public UpdateNotifiSettingResponse UpdateNotifiSetting(UpdateNotifiSettingRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNotifiSettingDataAccess.UpdateNotifiSetting(parameter);

                var response = new UpdateNotifiSettingResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateNotifiSettingResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public GetMasterDataSearchNotifiSettingResponse GetMasterDataSearchNotifiSetting(
            GetMasterDataSearchNotifiSettingRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNotifiSettingDataAccess.GetMasterDataSearchNotifiSetting(parameter);

                var response = new GetMasterDataSearchNotifiSettingResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListScreen = result.ListScreen
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataSearchNotifiSettingResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public SearchNotifiSettingResponse SearchNotifiSetting(SearchNotifiSettingRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNotifiSettingDataAccess.SearchNotifiSetting(parameter);

                var response = new SearchNotifiSettingResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListNotifiSetting = result.ListNotifiSetting
                };

                return response;
            }
            catch (Exception e)
            {
                return new SearchNotifiSettingResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public ChangeBackHourInternalResponse ChangeBackHourInternal(ChangeBackHourInternalRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNotifiSettingDataAccess.ChangeBackHourInternal(parameter);

                var response = new ChangeBackHourInternalResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new ChangeBackHourInternalResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public ChangeActiveResponse ChangeActive(ChangeActiveRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNotifiSettingDataAccess.ChangeActive(parameter);

                var response = new ChangeActiveResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new ChangeActiveResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public ChangeSendInternalResponse ChangeSendInternal(ChangeSendInternalRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNotifiSettingDataAccess.ChangeSendInternal(parameter);

                var response = new ChangeSendInternalResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new ChangeSendInternalResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public ChangeIsSystemResponse ChangeIsSystem(ChangeIsSystemRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNotifiSettingDataAccess.ChangeIsSystem(parameter);

                var response = new ChangeIsSystemResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new ChangeIsSystemResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public ChangeIsEmailResponse ChangeIsEmail(ChangeIsEmailRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNotifiSettingDataAccess.ChangeIsEmail(parameter);

                var response = new ChangeIsEmailResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new ChangeIsEmailResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public ChangeIsSmsResponse ChangeIsSms(ChangeIsSmsRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iNotifiSettingDataAccess.ChangeIsSms(parameter);

                var response = new ChangeIsSmsResponse
                {
                    StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new ChangeIsSmsResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }
    }
}
