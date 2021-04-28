using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetMasterDataOrderCreateParameter : BaseParameter
    {
        /*
         * 1: Tạo trực tiếp
         * 2: Tạo từ Khách hàng
         * 3: Tạo từ Báo giá
         * 4: Tạo từ Hợp đồng
         */ 
        public int CreateType { get; set; }  
        public Guid? CreateObjectId { get; set; }
    }
}
