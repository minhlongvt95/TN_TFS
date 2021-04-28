using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.NotifiSetting
{
    public class InforScreenEntityModel
    {
        public Guid InforScreenId { get; set; }
        public string InforScreenName { get; set; }
        public string InforScreenCode { get; set; }
        public Guid? ScreenId { get; set; }
        public bool IsDateTime { get; set; }
    }
}
