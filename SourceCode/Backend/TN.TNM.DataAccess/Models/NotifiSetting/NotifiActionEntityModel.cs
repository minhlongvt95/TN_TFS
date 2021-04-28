using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.NotifiSetting
{
    public class NotifiActionEntityModel
    {
        public Guid NotifiActionId { get; set; }
        public string NotifiActionName { get; set; }
        public string NotifiActionCode { get; set; }
        public Guid? ScreenId { get; set; }
    }
}
