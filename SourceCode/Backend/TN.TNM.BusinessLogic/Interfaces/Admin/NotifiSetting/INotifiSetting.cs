using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.NotifiSetting;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.NotifiSetting;

namespace TN.TNM.BusinessLogic.Interfaces.Admin.NotifiSetting
{
    public interface INotifiSetting
    {
        GetMasterDataNotifiSettingCreateResponse GetMasterDataNotifiSettingCreate(
            GetMasterDataNotifiSettingCreateRequest request);

        CreateNotifiSettingResponse CreateNotifiSetting(CreateNotifiSettingRequest request);

        GetMasterDataNotifiSettingDetailResponse GetMasterDataNotifiSettingDetail(
            GetMasterDataNotifiSettingDetailRequest request);

        UpdateNotifiSettingResponse UpdateNotifiSetting(UpdateNotifiSettingRequest request);

        GetMasterDataSearchNotifiSettingResponse GetMasterDataSearchNotifiSetting(
            GetMasterDataSearchNotifiSettingRequest request);

        SearchNotifiSettingResponse SearchNotifiSetting(SearchNotifiSettingRequest request);
        ChangeBackHourInternalResponse ChangeBackHourInternal(ChangeBackHourInternalRequest request);
        ChangeActiveResponse ChangeActive(ChangeActiveRequest request);
        ChangeSendInternalResponse ChangeSendInternal(ChangeSendInternalRequest request);
        ChangeIsSystemResponse ChangeIsSystem(ChangeIsSystemRequest request);
        ChangeIsEmailResponse ChangeIsEmail(ChangeIsEmailRequest request);
        ChangeIsSmsResponse ChangeIsSms(ChangeIsSmsRequest request);
    }
}
