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
    public static class CobotIotCommandFunction
    {
        [FunctionName("IotCommandFunction")]

        public static async Task<IActionResult> IotCommandFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest,
        ILogger log)
        {
            RespondModel respondModel = new RespondModel();
            string IOT_HUB_SERVICE_URL = Environment.GetEnvironmentVariable("IOT_HUB_SERVICE_URL");
            if (IOT_HUB_SERVICE_URL is null)
            {
                return new BadRequestObjectResult(respondModel.GetBadRequestRespondModel("IOT_HUB_SERVICE_URL is NULL"));
            }
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                IotCommandModel iotCommandModel = JsonConvert.DeserializeObject<IotCommandModel>(requestBody);
                if (iotCommandModel.DeviceId is null || iotCommandModel.Command is null)
                {
                    return new BadRequestObjectResult(respondModel
                        .GetBadRequestRespondModel("Error: Null value detected."));
                }
                if (!(iotCommandModel.Command.Equals("StartIotCommand") || iotCommandModel.Command.Equals("StopIotCommand")))
                {
                    return new BadRequestObjectResult(respondModel
                        .GetBadRequestRespondModel("Invalid command: The command you entered is not recognized or supported in this function"));
                }
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_SERVICE_URL);

                CloudToDeviceMethod cloudToDeviceMethod = new CloudToDeviceMethod(iotCommandModel.Command) { ResponseTimeout = TimeSpan.FromSeconds(iotCommandModel.ResponseTimeout) };
                cloudToDeviceMethod.SetPayloadJson(JsonConvert.SerializeObject("payload"));

                CloudToDeviceMethodResult cloudToDeviceMethodResult = await serviceClient.InvokeDeviceMethodAsync(iotCommandModel.DeviceId, cloudToDeviceMethod);
                respondModel.IotCommandModel = iotCommandModel;
                respondModel.ReportPayloadModel = ReportPayloadModel.GetFromCloudToDeviceMethodResult(cloudToDeviceMethodResult);

                if (respondModel.ReportPayloadModel.Payload.Status.Equals("COMMAND_EXECUTION_SEQUENCE_ERROR"))
                {
                    return new BadRequestObjectResult(respondModel.GetBadRequestRespondModel("Command cannot run because the execution sequence is incorrect."));
                }
                if (respondModel.ReportPayloadModel.Payload.Status.Equals("COBOT_CLIENT_EXECUTED"))
                {
                    return new OkObjectResult(respondModel.GetOkRequestRespondModel("The command was executed successfully."));
                }
            }
            catch (Exception exception)
            {
                ExceptionModel exceptionModel = ExceptionModel.GetFromException(exception);
                return new BadRequestObjectResult(respondModel.GetBadRequestRespondModel(exceptionModel.Message));
            }
            return new BadRequestObjectResult(respondModel.GetBadRequestRespondModel("Something happened."));
        }
        [FunctionName("ControlCommandFunction")]

        public static async Task<IActionResult> ControlCommandFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest,
        ILogger log)
        {
            RespondModel respondModel = new RespondModel();
            string IOT_HUB_SERVICE_URL = Environment.GetEnvironmentVariable("IOT_HUB_SERVICE_URL");
            if (IOT_HUB_SERVICE_URL is null)
            {
                return new BadRequestObjectResult("IOT_HUB_SERVICE_URL is NULL");
            }
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                IotCommandModel iotCommandModel = JsonConvert.DeserializeObject<IotCommandModel>(requestBody);
                if (iotCommandModel.DeviceId is null || iotCommandModel.Command is null)
                {
                    return new BadRequestObjectResult(respondModel
                        .GetBadRequestRespondModel("Error: Null value detected."));
                }
                if (!iotCommandModel.DeviceId.Equals("Cobot"))
                {
                    return new BadRequestObjectResult(respondModel
                        .GetBadRequestRespondModel("Invalid device: The device you entered is not recognized or supported in this function"));
                }
                if (!iotCommandModel.DeviceId.Equals("EnableControlCommand") ||
                    !iotCommandModel.Command.Equals("DisableControlCommand") ||
                    !iotCommandModel.Command.Equals("MoveJControlCommand") ||
                    !iotCommandModel.Command.Equals("MovePControlCommand") ||
                    !iotCommandModel.Command.Equals("MoveLControlCommand") ||
                    !iotCommandModel.Command.Equals("PauseControlCommand") ||
                    !iotCommandModel.Command.Equals("PlayControlCommand") ||
                    !iotCommandModel.Command.Equals("CloseSafetyPopupControlCommand") ||
                    !iotCommandModel.Command.Equals("UnlockProtectiveStopControlCommand") ||
                    !iotCommandModel.Command.Equals("OpenPopupControlCommand") ||
                    !iotCommandModel.Command.Equals("ClosePopupControlCommand") ||
                    !iotCommandModel.Command.Equals("PowerOnControlCommand") ||
                    !iotCommandModel.Command.Equals("PowerOffControlCommand") ||
                    !iotCommandModel.Command.Equals("StartFreeDriveControlCommand") ||
                    !iotCommandModel.Command.Equals("StopFreeDriveControlCommand"))
                {
                    return new BadRequestObjectResult(respondModel
                        .GetBadRequestRespondModel("Invalid command: The command you entered is not recognized or supported in this function"));
                }
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_SERVICE_URL);

                CloudToDeviceMethod cloudToDeviceMethod = new CloudToDeviceMethod(iotCommandModel.Command) { ResponseTimeout = TimeSpan.FromSeconds(iotCommandModel.ResponseTimeout) };
                cloudToDeviceMethod.SetPayloadJson(JsonConvert.SerializeObject("payload"));

                CloudToDeviceMethodResult cloudToDeviceMethodResult = await serviceClient.InvokeDeviceMethodAsync(iotCommandModel.DeviceId, cloudToDeviceMethod);
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
            }
            catch (Exception e)
            {
                if (e.Message.Contains("The operation failed because the requested device isn't online."))
                {
                    //return new BadRequestObjectResult(respondModel.Get_Device_Not_Online_RespondModel());
                }
                else if (e.Message.Contains("Timed out waiting for the direct method response from the device."))
                {
                    //return new BadRequestObjectResult(respondModel.Get_Timed_Out_RespondModel());
                }
                else
                {
                    return new BadRequestObjectResult(respondModel.GetBadRequestRespondModel(e.Message.ToString()));
                }
            }
            return new BadRequestObjectResult(respondModel.GetBadRequestRespondModel("Something happened."));
        }
    }
}
