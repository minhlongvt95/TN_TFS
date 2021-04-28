using System;
using System.ServiceModel;
using TN.TNM.Common.svcutil_generated;

namespace TN.TNM.Common
{
    public class SAPIService
    {
        private static readonly EndpointAddress Endpoint = new EndpointAddress("http://sis.ispeaking.edu.vn/SAPI.asmx?wsdl");
        private static readonly BasicHttpBinding Binding = new BasicHttpBinding
        {
            MaxReceivedMessageSize = 2147483647,
            MaxBufferSize = 2147483647
        };

        public static string GetTeacherSummary(DateTime fTime,DateTime eTime)
        {
            string Result = string.Empty;
            try
            {
                using (var proxy = new GenericProxy<SAPISoap>(Binding, Endpoint))
                {
                    GetTeacherSummaryResponse response = new GetTeacherSummaryResponse();
                    GetTeacherSummaryRequestBody Body = new GetTeacherSummaryRequestBody();
                    GetTeacherSummaryRequest request = new GetTeacherSummaryRequest(Body);
                    request.Body.key = "d41d8cd98f00b204e9800998ecf8427e";
                    request.Body.fTime = fTime;
                    request.Body.eTime = eTime;
                    response = proxy.Execute(c => c.GetTeacherSummary(request));
                    Result = response.Body.GetTeacherSummaryResult;
                }
            }
            catch (FaultException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
            return Result;
        }
    }
}
