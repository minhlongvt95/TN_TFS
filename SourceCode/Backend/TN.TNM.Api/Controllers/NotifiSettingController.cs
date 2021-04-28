using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.NotifiSetting;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.NotifiSetting;
using TN.TNM.BusinessLogic.Messages.Requests.Customer;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.NotifiSetting;

namespace TN.TNM.Api.Controllers
{
    public class NotifiSettingController : Controller
    {
        private readonly INotifiSetting _iNotifiSetting;
        public NotifiSettingController(INotifiSetting iNotifiSetting)
        {
            this._iNotifiSetting = iNotifiSetting;
        }

        [HttpPost]
        [Route("api/notifisetting/getMasterDataNotifiSettingCreate")]
        [Authorize(Policy = "Member")]
        public GetMasterDataNotifiSettingCreateResponse GetMasterDataNotifiSettingCreate(
            [FromBody]GetMasterDataNotifiSettingCreateRequest request)
        {
            return this._iNotifiSetting.GetMasterDataNotifiSettingCreate(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/createNotifiSetting")]
        [Authorize(Policy = "Member")]
        public CreateNotifiSettingResponse CreateNotifiSetting(
            [FromBody]CreateNotifiSettingRequest request)
        {
            return this._iNotifiSetting.CreateNotifiSetting(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/getMasterDataNotifiSettingDetail")]
        [Authorize(Policy = "Member")]
        public GetMasterDataNotifiSettingDetailResponse GetMasterDataNotifiSettingDetail(
            [FromBody]GetMasterDataNotifiSettingDetailRequest request)
        {
            return this._iNotifiSetting.GetMasterDataNotifiSettingDetail(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/updateNotifiSetting")]
        [Authorize(Policy = "Member")]
        public UpdateNotifiSettingResponse UpdateNotifiSetting(
            [FromBody]UpdateNotifiSettingRequest request)
        {
            return this._iNotifiSetting.UpdateNotifiSetting(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/getMasterDataSearchNotifiSetting")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchNotifiSettingResponse GetMasterDataSearchNotifiSetting(
            [FromBody]GetMasterDataSearchNotifiSettingRequest request)
        {
            return this._iNotifiSetting.GetMasterDataSearchNotifiSetting(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/searchNotifiSetting")]
        [Authorize(Policy = "Member")]
        public SearchNotifiSettingResponse SearchNotifiSetting(
            [FromBody]SearchNotifiSettingRequest request)
        {
            return this._iNotifiSetting.SearchNotifiSetting(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/changeBackHourInternal")]
        [Authorize(Policy = "Member")]
        public ChangeBackHourInternalResponse ChangeBackHourInternal(
            [FromBody]ChangeBackHourInternalRequest request)
        {
            return this._iNotifiSetting.ChangeBackHourInternal(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/changeActive")]
        [Authorize(Policy = "Member")]
        public ChangeActiveResponse ChangeActive(
            [FromBody]ChangeActiveRequest request)
        {
            return this._iNotifiSetting.ChangeActive(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/changeSendInternal")]
        [Authorize(Policy = "Member")]
        public ChangeSendInternalResponse ChangeSendInternal(
            [FromBody]ChangeSendInternalRequest request)
        {
            return this._iNotifiSetting.ChangeSendInternal(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/changeIsSystem")]
        [Authorize(Policy = "Member")]
        public ChangeIsSystemResponse ChangeIsSystem(
            [FromBody]ChangeIsSystemRequest request)
        {
            return this._iNotifiSetting.ChangeIsSystem(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/changeIsEmail")]
        [Authorize(Policy = "Member")]
        public ChangeIsEmailResponse ChangeIsEmail(
            [FromBody]ChangeIsEmailRequest request)
        {
            return this._iNotifiSetting.ChangeIsEmail(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/changeIsSms")]
        [Authorize(Policy = "Member")]
        public ChangeIsSmsResponse ChangeIsSms(
            [FromBody]ChangeIsSmsRequest request)
        {
            return this._iNotifiSetting.ChangeIsSms(request);
        }
    }
}
