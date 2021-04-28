using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.Common.NotificationSetting
{
    public static class TypeCondition
    {
        public const int EQUAL = 1;
        public const int NOT_EQUAL = 2;
        public const int CONTAINS = 3;
        public const int NOT_CONTAINS = 4;
        public const int NULL = 5;
        public const int NOT_NULL = 6;
        public const int DATE_EQUAL = 7; //Là
        public const int TODAY = 8;     //Hôm nay
        public const int IN_WEEK = 9;   //Tuần này
        public const int IN_MONTH = 10; //Tháng này
        public const int IN_YEAR = 11;  //Năm này
        public const int NEXT_WEEK = 12; //Tuần tới
        public const int NEXT_MONTH = 13; //Tháng tới
        public const int NEXT_YEAR = 14; //Năm tới
        public const int PRE_DAY = 15; //Trước ngày
        public const int AFT_DAY = 16; //Sau ngày
    }
}
