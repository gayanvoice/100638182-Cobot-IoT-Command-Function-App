using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CobotIoTCommandFunctionApp.Model
{
    public class RespondModel
    {
        private Stopwatch stopwatch;
        public string Message { get; set; }
        public double Duration { get; set; }
        public ReportPayloadModel ReportPayloadModel{ get; set; }
        public IotCommandModel IotCommandModel { get; set; }
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
    }
}