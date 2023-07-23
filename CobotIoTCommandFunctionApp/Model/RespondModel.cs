using Newtonsoft.Json;
using System.Diagnostics;

namespace CobotIoTCommandFunctionApp.Model
{
    public class RespondModel
    {
        private Stopwatch stopwatch;
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("duration")]
        public double Duration { get; set; }
        [JsonProperty("report_payload_model")]
        public ReportPayloadModel ReportPayloadModel{ get; set; }
        [JsonProperty("iot_command_model")]
        public IoTCommandModel IoTCommandModel { get; set; }
        public RespondModel()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }
        public RespondModel GetOkRequestRespondModel(string text)
        {
            stopwatch.Stop();
            Message =  text;
            Duration = stopwatch.Elapsed.TotalMilliseconds;
            return this;
        }
        public RespondModel GetBadRequestRespondModel(string text)
        {
            stopwatch.Stop();
            Message = text;
            Duration = stopwatch.Elapsed.TotalMilliseconds;
            return this;
        }
        public RespondModel Get_Invalid_IOT_HUB_SERVICE_URL_RespondModel()
        {
            stopwatch.Stop();
            Message = "Request Failed. Invalid IOT_HUB_SERVICE_URL.";
            Duration = stopwatch.Elapsed.TotalMilliseconds;
            return this;
        }
        public RespondModel Get_Invalid_JSON_Request_Body_RespondModel()
        {
            stopwatch.Stop();
            Message = "Request Failed. Invalid JSON request body.";
            Duration = stopwatch.Elapsed.TotalMilliseconds;
            return this;
        }
        public RespondModel Get_Device_Not_Online_RespondModel()
        {
            stopwatch.Stop();
            Message = "Request Failed. The operation failed because the requested device isn't online.";
            Duration = stopwatch.Elapsed.TotalMilliseconds;
            return this;
        }
        public RespondModel Get_Timed_Out_RespondModel()
        {
            stopwatch.Stop();
            Message = "Request Failed. Timed out waiting for the direct method response from the device.";
            Duration = stopwatch.Elapsed.TotalMilliseconds;
            return this;
        }
    }
}