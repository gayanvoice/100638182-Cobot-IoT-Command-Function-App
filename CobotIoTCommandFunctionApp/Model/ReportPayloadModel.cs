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
            double startPerfCounter = (double) jsonObject["_start_perf_counter"];
            double endPerfCounter = (double) jsonObject["_end_perf_counter"];
            PayloadModel payloadModel = new PayloadModel();
            payloadModel.Status = (string) jsonObject["_status"];
            payloadModel.Message = (string) jsonObject["_message"];
            payloadModel.Duration = Math.Round(endPerfCounter - startPerfCounter, 4);
            ReportPayloadModel reportPayloadModel = new ReportPayloadModel();
            reportPayloadModel.Result = cloudToDeviceMethodResult.Status;
            reportPayloadModel.Payload = payloadModel;
            return reportPayloadModel;
        }
    }
}