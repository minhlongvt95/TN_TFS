using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.NotifiSetting
{
    public class NotifiConditionEntityModel
    {
        public Guid NotifiConditionId { get; set; }
        public string NotifiConditionName { get; set; }
        public int TypeValue { get; set; }
        public int TypeCondition { get; set; }
        public Guid? ScreenId { get; set; }
    }
}
