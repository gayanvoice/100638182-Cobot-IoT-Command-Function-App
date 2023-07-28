using Newtonsoft.Json.Linq;
using System;

namespace CobotIotCommandFunctionApp.Model.Error
{
    public class ExceptionModel
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public static ExceptionModel GetFromException(Exception exception)
        {
            JObject exceptionObject = JObject.Parse(exception.Message);
            JObject messageObject = JObject.Parse((string)exceptionObject["Message"]);

            ExceptionModel exceptionModel = new ExceptionModel();
            exceptionModel.ErrorCode = (int)messageObject["errorCode"];
            exceptionModel.Message = (string)messageObject["message"];
            return exceptionModel;
        }
    }
}