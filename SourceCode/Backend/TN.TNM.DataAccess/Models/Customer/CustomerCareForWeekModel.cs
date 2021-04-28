using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Customer
{
    public class CustomerCareForWeekModel
    {
        public Guid CustomerCareId { get; set; }
        public Guid EmployeeCharge { get; set; }
        public string Title { get; set; }
        public int Type { get; set; } //Loại chương trình CSKH (1: Gửi SMS, 2: Gửi email, 3: Tặng quà, 4: Gọi điện)
        public int FeedBackStatus { get; set; } //Trạng thái phản hồi của chương trình CSKH (0: Không cần feedback, 1: Đã phản hồi, 2: Chưa phản hồi)
        public string Background { get; set; }
        public string SubTitle { get; set; }
        public DateTime ActiveDate { get; set; } //Với Gửi SMS, Email là ngày gửi; Với Tặng quà, Gọi điện là ngày kích hoạt;
    }
}
