using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetSerialResult:BaseResult
    {
        public List<Serial> lstSerial { get; set;}
    }
}
