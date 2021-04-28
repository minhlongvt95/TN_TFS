using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting;
using TN.TNM.DataAccess.Messages.Results.Admin.NotifiSetting;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface INotifiSettingDataAccess
    {
        GetMasterDataNotifiSettingCreateResult GetMasterDataNotifiSettingCreate(
            GetMasterDataNotifiSettingCreateParameter parameter);

        CreateNotifiSettingResult CreateNotifiSetting(CreateNotifiSettingParameter parameter);

        GetMasterDataNotifiSettingDetailResult GetMasterDataNotifiSettingDetail(
            GetMasterDataNotifiSettingDetailParameter parameter);

        UpdateNotifiSettingResult UpdateNotifiSetting(UpdateNotifiSettingParameter parameter);

        GetMasterDataSearchNotifiSettingResult GetMasterDataSearchNotifiSetting(
            GetMasterDataSearchNotifiSettingParameter parameter);

        SearchNotifiSettingResult SearchNotifiSetting(SearchNotifiSettingParameter parameter);
        ChangeBackHourInternalResult ChangeBackHourInternal(ChangeBackHourInternalParameter parameter);
        ChangeActiveResult ChangeActive(ChangeActiveParameter parameter);
        ChangeSendInternalResult ChangeSendInternal(ChangeSendInternalParameter parameter);
        ChangeIsSystemResult ChangeIsSystem(ChangeIsSystemParameter parameter);
        ChangeIsEmailResult ChangeIsEmail(ChangeIsEmailParameter parameter);
        ChangeIsSmsResult ChangeIsSms(ChangeIsSmsParameter parameter);
    }
}
