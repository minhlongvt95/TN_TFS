using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.NotifiSetting;

namespace TN.TNM.DataAccess.Messages.Results.Admin.NotifiSetting
{
    public class GetMasterDataNotifiSettingDetailResult : BaseResult
    {
        public List<ScreenEntityModel> ListScreen { get; set; }
        public List<NotifiActionEntityModel> ListNotifiAction { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<InforScreenEntityModel> ListInforScreen { get; set; }
        public NotifiSettingEntityModel NotifiSetting { get; set; }
        public List<NotifiSpecialEntityModel> ListNotifiSpecial { get; set; }
        public List<NotifiSettingTokenEntityModel> ListNotifiSettingToken { get; set; }
    }
}
