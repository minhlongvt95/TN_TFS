using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.NotifiSetting
{
    public class NotifiSettingTokenEntityModel
    {
        public Guid NotifiSettingTokenId { get; set; }
        public string TokenCode { get; set; }
        public string TokenLabel { get; set; }
        public Guid? ScreenId { get; set; }
    }
}
