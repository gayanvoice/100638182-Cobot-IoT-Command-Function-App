using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CobotIoTCommandFunctionApp.Model;
using Microsoft.Azure.Devices;

namespace CobotIoTCommandFunctionApp
{
    public static class CobotIoTCommandFunction
    {
        [FunctionName("IoTCommandFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest,
        ILogger log)
        {
            RespondModel respondModel = new RespondModel();
            string IOT_HUB_SERVICE_URL = Environment.GetEnvironmentVariable("IOT_HUB_SERVICE_URL");
            if (IOT_HUB_SERVICE_URL is null)
            {
                return new BadRequestObjectResult(respondModel.Get_Invalid_IOT_HUB_SERVICE_URL_RespondModel());
            }
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                IoTCommandModel iotCommandModel = JsonConvert.DeserializeObject<IoTCommandModel>(requestBody);
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_SERVICE_URL);

                CloudToDeviceMethod cloudToDeviceMethod = new CloudToDeviceMethod(iotCommandModel.IoTCommand) { ResponseTimeout = TimeSpan.FromSeconds(20) };
                cloudToDeviceMethod.SetPayloadJson(JsonConvert.SerializeObject(iotCommandModel.IoTPayload));

                CloudToDeviceMethodResult cloudToDeviceMethodResult = await serviceClient.InvokeDeviceMethodAsync(iotCommandModel.IoTDeviceId, cloudToDeviceMethod);
                respondModel.IoTCommandModel = iotCommandModel;
                respondModel.ReportPayloadModel = ReportPayloadModel.GetFromCloudToDeviceMethodResult(cloudToDeviceMethodResult);

                if (respondModel.ReportPayloadModel.Payload.Status.Equals("COMMAND_EXECUTION_SEQUENCE_ERROR"))
                {
                    return new BadRequestObjectResult(respondModel.GetBadRequestRespondModel("Command cannot run because the execution sequence is incorrect."));
                }
                if (respondModel.ReportPayloadModel.Payload.Status.Equals("COBOT_CLIENT_EXECUTED"))
                {
                    return new OkObjectResult(respondModel.GetOkRequestRespondModel("The command was executed successfully."));
                }
                return new BadRequestObjectResult(respondModel.GetBadRequestRespondModel("Something happened."));
            }
            catch (Exception e)
            {
                if (e.Message.Contains("The operation failed because the requested device isn't online."))
                {
                    return new BadRequestObjectResult(respondModel.Get_Device_Not_Online_RespondModel());
                }
                else if (e.Message.Contains("Timed out waiting for the direct method response from the device."))
                {
                    return new BadRequestObjectResult(respondModel.Get_Timed_Out_RespondModel());
                }
                else
                {
                    return new BadRequestObjectResult(respondModel.GetBadRequestRespondModel(e.Message.ToString()));
                }
            }           
           
        }
    }
}
