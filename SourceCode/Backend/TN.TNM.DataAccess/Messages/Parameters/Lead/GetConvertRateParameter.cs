using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class GetConvertRateParameter : BaseParameter
    {
      public int? Month { get; set; }
      public int? Year { get; set; }
    }
}
