using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Azure.Devices;
using Newtonsoft.Json.Linq;

namespace CobotIotCommandFunctionApp.Model.Response
{
    public class PlayControlCommandResponseModel
    {
        private Stopwatch stopwatch;
        public string Message { get; set; }
        public double Duration { get; set; }
        public CommandResponseModel CommandResponse { get; set; }
        public CommandRequestModel CommandRequest { get; set; }
        public PlayControlCommandResponseModel()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }
        public PlayControlCommandResponseModel GetOkRequestRespondModel(string text)
        {
            stopwatch.Stop();
            Message = text;
            Duration = stopwatch.Elapsed.TotalMilliseconds;
            return this;
        }
        public Dictionary<string, object> GetBadRequestRespondModel(string text)
        {
            stopwatch.Stop();
            Message = text;
            Duration = stopwatch.Elapsed.TotalMilliseconds;
            return GetType()
                .GetProperties()
                .Where(property => property.GetValue(this) != null)
                .ToDictionary(property => FirstLetterToLower(property.Name), property => property.GetValue(this));
        }
        public static string FirstLetterToLower(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToLower(input[0]) + input.Substring(1);
        }
        public class CommandRequestModel
        {
            public string DeviceId { get; set; }
            public double ResponseTimeout { get; set; } = 20.0;
        }
        public class CommandResponseModel
        {
            public double Result { get; set; }
            public PayloadModel Payload { get; set; }
            public static CommandResponseModel GetCommandResponseModel(
                CloudToDeviceMethodResult cloudToDeviceMethodResult)
            {
                PayloadModel payloadModel = PayloadModel.GetFromCloudToDeviceMethodResult(cloudToDeviceMethodResult);
                CommandResponseModel commandResponseModel = new CommandResponseModel();
                commandResponseModel.Result = cloudToDeviceMethodResult.Status;
                commandResponseModel.Payload = payloadModel;
                return commandResponseModel;
            }
            public class PayloadModel
            {
                public string Status { get; set; }
                public string LogText { get; set; }
                public double Duration { get; set; }
                public object RobotMode { get; set; }
                public object RobotStatus { get; set; }
                public static PayloadModel GetFromCloudToDeviceMethodResult(
                    CloudToDeviceMethodResult cloudToDeviceMethodResult)
                {
                    string jsonString = cloudToDeviceMethodResult.GetPayloadAsJson().Trim('"').Replace("\\\"", "\"");
                    JObject jsonObject = JObject.Parse(jsonString);
                    PayloadModel payloadModel = new PayloadModel();
                    payloadModel.Status = (string)jsonObject["status"];
                    payloadModel.LogText = (string)jsonObject["log_text"];
                    payloadModel.Duration = (double)jsonObject["duration"];
                    payloadModel.RobotMode = jsonObject["robot_mode"];
                    payloadModel.RobotStatus = jsonObject["robot_status"];
                    return payloadModel;
                }
            }

        }
    }
}