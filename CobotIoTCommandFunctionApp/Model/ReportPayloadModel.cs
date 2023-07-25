using Microsoft.Azure.Devices;
using Newtonsoft.Json.Linq;

namespace CobotIoTCommandFunctionApp.Model
{
    public class ReportPayloadModel
    {
        public double Result { get; set; }
        public PayloadModel Payload { get; set; }
        public class PayloadModel
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public double Duration { get; set; }
        }
        public static ReportPayloadModel GetFromCloudToDeviceMethodResult(CloudToDeviceMethodResult cloudToDeviceMethodResult)
        {
            string jsonString = cloudToDeviceMethodResult.GetPayloadAsJson().Trim('"').Replace("\\\"", "\"");
            JObject jsonObject = JObject.Parse(jsonString);
            PayloadModel payloadModel = new PayloadModel();
            payloadModel.Status = (string) jsonObject["_status"];
            payloadModel.Message = (string) jsonObject["_message"];
            payloadModel.Duration = (double) jsonObject["_duration"];
            ReportPayloadModel reportPayloadModel = new ReportPayloadModel();
            reportPayloadModel.Result = cloudToDeviceMethodResult.Status;
            reportPayloadModel.Payload = payloadModel;
            return reportPayloadModel;
        }
    }
}