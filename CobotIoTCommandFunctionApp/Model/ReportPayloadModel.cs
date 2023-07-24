using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace CobotIoTCommandFunctionApp.Model
{
    public class ReportPayloadModel
    {
        [JsonProperty("status")]
        public double Result { get; set; }
        [JsonProperty("payload_model")]
        public PayloadModel Payload { get; set; }
        public class PayloadModel
        {
            [JsonProperty("status")]
            public string Status { get; set; }
            [JsonProperty("message")]
            public string Message { get; set; }
            [JsonProperty("duration")]
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