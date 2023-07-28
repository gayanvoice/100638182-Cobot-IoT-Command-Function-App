using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using CobotIotCommandFunctionApp.Model.Error;
using CobotIotCommandFunctionApp.Model.Response;

namespace CobotIoTCommandFunctionApp
{
    public static class CobotCommandFunction
    {
        private const string IOT_HUB_SERVICE_URL_ENVIRONMENT_VARIABLE = "IOT_HUB_SERVICE_URL";
        private const string START_IOT_COMMAND_VARIABLE = "StartIotCommand";
        private const string STOP_IOT_COMMAND_VARIABLE = "StopIotCommand";
        private const string ENABLE_CONTROL_COMMAND_VARIABLE = "EnableControlCommand";
        private const string DISABLE_CONTROL_COMMAND_VARIABLE = "DisableControlCommand";
        private const string START_FREE_DRIVE_CONTROL_COMMAND_VARIABLE = "StartFreeDriveControlCommand";
        private const string STOP_FREE_DRIVE_CONTROL_COMMAND_VARIABLE = "StopFreeDriveControlCommand";
        private const string POWER_ON_CONTROL_COMMAND_VARIABLE = "PowerOnControlCommand";
        private const string POWER_OFF_CONTROL_COMMAND_VARIABLE = "PowerOffControlCommand";
        private const string PLAY_CONTROL_COMMAND_VARIABLE = "PlayControlCommand";
        private const string PAUSE_CONTROL_COMMAND_VARIABLE = "PauseControlCommand";
        private const string CLOSE_SAFETY_POPUP_CONTROL_COMMAND_VARIABLE = "CloseSafetyPopupControlCommand";
        private const string UNLOCK_PROTECTIVE_STOP_CONTROL_COMMAND_VARIABLE = "UnlockProtectiveStopControlCommand";
        private const string OPEN_POPUP_CONTROL_COMMAND_VARIABLE = "OpenPopupControlCommand";
        private const string CLOSE_POPUP_CONTROL_COMMAND_VARIABLE = "ClosePopupControlCommand";

        private const string COMMAND_EXECUTION_SEQUENCE_ERROR_VARIABLE = "COMMAND_EXECUTION_SEQUENCE_ERROR";
        private const string COBOT_CLIENT_EXECUTED_VARIABLE = "COBOT_CLIENT_EXECUTED";

        private const string IOT_HUB_SERVICE_URL_IS_NULL_MESSAGE = "IOT_HUB_SERVICE_URL is NULL";
        private const string ERROR_NULL_VALUES_DETECTED_MESSAGE = "Error: Null value detected.";
        private const string INCORRECT_SEQUENCE_COMMAND_CANNOT_RUN_MESSAGE = "Command cannot run because the execution sequence is incorrect.";
        private const string COMMAND_EXECUTED_SUCCESSFULLY_MESSAGE = "The command was executed successfully.";
        private const string SOMETHING_HAPPENED_MESSAGE = "Something happened.";

        [FunctionName("StartIotCommandFunction")]
        public static async Task<IActionResult> StartIotCommandFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest, ILogger log)
        {
            StartIotCommandResponseModel startIotCommandResponseModel = new StartIotCommandResponseModel();
            
            string IOT_HUB_SERVICE_URL = Environment.GetEnvironmentVariable(IOT_HUB_SERVICE_URL_ENVIRONMENT_VARIABLE);
            if (IOT_HUB_SERVICE_URL is null)
            {
                return new BadRequestObjectResult(startIotCommandResponseModel.GetBadRequestRespondModel(IOT_HUB_SERVICE_URL_IS_NULL_MESSAGE));
            }
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                StartIotCommandResponseModel.CommandRequestModel startIotCommandRequestModel = JsonConvert.DeserializeObject<StartIotCommandResponseModel.CommandRequestModel>(requestBody);
                if (startIotCommandRequestModel.DeviceId is null)
                {
                    return new BadRequestObjectResult(startIotCommandResponseModel.GetBadRequestRespondModel(ERROR_NULL_VALUES_DETECTED_MESSAGE));
                }
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_SERVICE_URL);
                CloudToDeviceMethod cloudToDeviceMethod = new CloudToDeviceMethod(START_IOT_COMMAND_VARIABLE);
                cloudToDeviceMethod.ResponseTimeout = TimeSpan.FromSeconds(startIotCommandRequestModel.ResponseTimeout);
                CloudToDeviceMethodResult cloudToDeviceMethodResult = await serviceClient
                    .InvokeDeviceMethodAsync(startIotCommandRequestModel.DeviceId, cloudToDeviceMethod);
                startIotCommandResponseModel.CommandRequest = startIotCommandRequestModel;
                startIotCommandResponseModel.CommandResponse = StartIotCommandResponseModel.CommandResponseModel.GetCommandResponseModel(cloudToDeviceMethodResult);
                if (startIotCommandResponseModel.CommandResponse.Payload.Status.Equals(COMMAND_EXECUTION_SEQUENCE_ERROR_VARIABLE))
                {
                    return new BadRequestObjectResult(startIotCommandResponseModel.GetBadRequestRespondModel(INCORRECT_SEQUENCE_COMMAND_CANNOT_RUN_MESSAGE));
                }
                if (startIotCommandResponseModel.CommandResponse.Payload.Status.Equals(COBOT_CLIENT_EXECUTED_VARIABLE))
                {
                    return new OkObjectResult(startIotCommandResponseModel.GetOkRequestRespondModel(COMMAND_EXECUTED_SUCCESSFULLY_MESSAGE));
                }
            }
            catch (Exception exception)
            {
                ExceptionModel exceptionModel = ExceptionModel.GetFromException(exception);
                return new BadRequestObjectResult(startIotCommandResponseModel.GetBadRequestRespondModel(exceptionModel.Message));
            }
            return new BadRequestObjectResult(startIotCommandResponseModel.GetBadRequestRespondModel(SOMETHING_HAPPENED_MESSAGE));
        }
        [FunctionName("StopIotCommandFunction")]
        public static async Task<IActionResult> StopIotCommandFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest, ILogger log)
        {
            StopIotCommandResponseModel stopIotCommandResponseModel = new StopIotCommandResponseModel();

            string IOT_HUB_SERVICE_URL = Environment.GetEnvironmentVariable(IOT_HUB_SERVICE_URL_ENVIRONMENT_VARIABLE);
            if (IOT_HUB_SERVICE_URL is null)
            {
                return new BadRequestObjectResult(stopIotCommandResponseModel.GetBadRequestRespondModel(IOT_HUB_SERVICE_URL_IS_NULL_MESSAGE));
            }
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                StopIotCommandResponseModel.CommandRequestModel stopIotCommandRequestModel = JsonConvert.DeserializeObject<StopIotCommandResponseModel.CommandRequestModel>(requestBody);
                if (stopIotCommandRequestModel.DeviceId is null)
                {
                    return new BadRequestObjectResult(stopIotCommandResponseModel.GetBadRequestRespondModel(ERROR_NULL_VALUES_DETECTED_MESSAGE));
                }
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_SERVICE_URL);
                CloudToDeviceMethod cloudToDeviceMethod = new CloudToDeviceMethod(STOP_IOT_COMMAND_VARIABLE);
                cloudToDeviceMethod.ResponseTimeout = TimeSpan.FromSeconds(stopIotCommandRequestModel.ResponseTimeout);
                CloudToDeviceMethodResult cloudToDeviceMethodResult = await serviceClient.InvokeDeviceMethodAsync(stopIotCommandRequestModel.DeviceId, cloudToDeviceMethod);
                stopIotCommandResponseModel.CommandRequest = stopIotCommandRequestModel;
                stopIotCommandResponseModel.CommandResponse = StopIotCommandResponseModel.CommandResponseModel.GetCommandResponseModel(cloudToDeviceMethodResult);
                if (stopIotCommandResponseModel.CommandResponse.Payload.Status.Equals(COMMAND_EXECUTION_SEQUENCE_ERROR_VARIABLE))
                {
                    return new BadRequestObjectResult(stopIotCommandResponseModel.GetBadRequestRespondModel(INCORRECT_SEQUENCE_COMMAND_CANNOT_RUN_MESSAGE));
                }
                if (stopIotCommandResponseModel.CommandResponse.Payload.Status.Equals(COBOT_CLIENT_EXECUTED_VARIABLE))
                {
                    return new OkObjectResult(stopIotCommandResponseModel.GetOkRequestRespondModel(COMMAND_EXECUTED_SUCCESSFULLY_MESSAGE));
                }
            }
            catch (Exception exception)
            {
                ExceptionModel exceptionModel = ExceptionModel.GetFromException(exception);
                return new BadRequestObjectResult(stopIotCommandResponseModel.GetBadRequestRespondModel(exceptionModel.Message));
            }
            return new BadRequestObjectResult(stopIotCommandResponseModel.GetBadRequestRespondModel(SOMETHING_HAPPENED_MESSAGE));
        }
        [FunctionName("EnableControlCommandFunction")]
        public static async Task<IActionResult> EnableControlCommandFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest, ILogger log)
        {
            EnableControlCommandResponseModel enableControlCommandResponseModel = new EnableControlCommandResponseModel();

            string IOT_HUB_SERVICE_URL = Environment.GetEnvironmentVariable(IOT_HUB_SERVICE_URL_ENVIRONMENT_VARIABLE);
            if (IOT_HUB_SERVICE_URL is null)
            {
                return new BadRequestObjectResult(enableControlCommandResponseModel.GetBadRequestRespondModel(IOT_HUB_SERVICE_URL_IS_NULL_MESSAGE));
            }
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                EnableControlCommandResponseModel.CommandRequestModel enableControlCommandRequestModel = JsonConvert
                    .DeserializeObject<EnableControlCommandResponseModel.CommandRequestModel>(requestBody);
                if (enableControlCommandRequestModel.DeviceId is null)
                {
                    return new BadRequestObjectResult(enableControlCommandResponseModel
                        .GetBadRequestRespondModel(ERROR_NULL_VALUES_DETECTED_MESSAGE));
                }
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_SERVICE_URL);
                CloudToDeviceMethod cloudToDeviceMethod = new CloudToDeviceMethod(ENABLE_CONTROL_COMMAND_VARIABLE);
                cloudToDeviceMethod.ResponseTimeout = TimeSpan.FromSeconds(enableControlCommandRequestModel.ResponseTimeout);
                CloudToDeviceMethodResult cloudToDeviceMethodResult = await serviceClient
                    .InvokeDeviceMethodAsync(enableControlCommandRequestModel.DeviceId, cloudToDeviceMethod);
                enableControlCommandResponseModel.CommandRequest = enableControlCommandRequestModel;
                enableControlCommandResponseModel.CommandResponse = EnableControlCommandResponseModel.CommandResponseModel
                    .GetCommandResponseModel(cloudToDeviceMethodResult);
                if (enableControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COMMAND_EXECUTION_SEQUENCE_ERROR_VARIABLE))
                {
                    return new BadRequestObjectResult(enableControlCommandResponseModel
                        .GetBadRequestRespondModel(INCORRECT_SEQUENCE_COMMAND_CANNOT_RUN_MESSAGE));
                }
                if (enableControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COBOT_CLIENT_EXECUTED_VARIABLE))
                {
                    return new OkObjectResult(enableControlCommandResponseModel.GetOkRequestRespondModel(COMMAND_EXECUTED_SUCCESSFULLY_MESSAGE));
                }
            }
            catch (Exception exception)
            {
                ExceptionModel exceptionModel = ExceptionModel.GetFromException(exception);
                return new BadRequestObjectResult(enableControlCommandResponseModel.GetBadRequestRespondModel(exceptionModel.Message));
            }
            return new BadRequestObjectResult(enableControlCommandResponseModel.GetBadRequestRespondModel(SOMETHING_HAPPENED_MESSAGE));
        }
        [FunctionName("DisableControlCommandFunction")]
        public static async Task<IActionResult> DisableControlCommandFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest, ILogger log)
        {
            DisableControlCommandResponseModel disableControlCommandResponseModel = new DisableControlCommandResponseModel();

            string IOT_HUB_SERVICE_URL = Environment.GetEnvironmentVariable(IOT_HUB_SERVICE_URL_ENVIRONMENT_VARIABLE);
            if (IOT_HUB_SERVICE_URL is null)
            {
                return new BadRequestObjectResult(disableControlCommandResponseModel.GetBadRequestRespondModel(IOT_HUB_SERVICE_URL_IS_NULL_MESSAGE));
            }
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                DisableControlCommandResponseModel.CommandRequestModel disableControlCommandRequestModel = JsonConvert
                    .DeserializeObject<DisableControlCommandResponseModel.CommandRequestModel>(requestBody);
                if (disableControlCommandRequestModel.DeviceId is null)
                {
                    return new BadRequestObjectResult(disableControlCommandResponseModel
                        .GetBadRequestRespondModel(ERROR_NULL_VALUES_DETECTED_MESSAGE));
                }
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_SERVICE_URL);
                CloudToDeviceMethod cloudToDeviceMethod = new CloudToDeviceMethod(DISABLE_CONTROL_COMMAND_VARIABLE);
                cloudToDeviceMethod.ResponseTimeout = TimeSpan.FromSeconds(disableControlCommandRequestModel.ResponseTimeout);
                CloudToDeviceMethodResult cloudToDeviceMethodResult = await serviceClient
                    .InvokeDeviceMethodAsync(disableControlCommandRequestModel.DeviceId, cloudToDeviceMethod);
                disableControlCommandResponseModel.CommandRequest = disableControlCommandRequestModel;
                disableControlCommandResponseModel.CommandResponse = DisableControlCommandResponseModel.CommandResponseModel
                    .GetCommandResponseModel(cloudToDeviceMethodResult);
                if (disableControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COMMAND_EXECUTION_SEQUENCE_ERROR_VARIABLE))
                {
                    return new BadRequestObjectResult(disableControlCommandResponseModel
                        .GetBadRequestRespondModel(INCORRECT_SEQUENCE_COMMAND_CANNOT_RUN_MESSAGE));
                }
                if (disableControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COBOT_CLIENT_EXECUTED_VARIABLE))
                {
                    return new OkObjectResult(disableControlCommandResponseModel.GetOkRequestRespondModel(COMMAND_EXECUTED_SUCCESSFULLY_MESSAGE));
                }
            }
            catch (Exception exception)
            {
                ExceptionModel exceptionModel = ExceptionModel.GetFromException(exception);
                return new BadRequestObjectResult(disableControlCommandResponseModel.GetBadRequestRespondModel(exceptionModel.Message));
            }
            return new BadRequestObjectResult(disableControlCommandResponseModel.GetBadRequestRespondModel(SOMETHING_HAPPENED_MESSAGE));
        }
        [FunctionName("StartFreeDriveControlCommandFunction")]
        public static async Task<IActionResult> StartFreeDriveControlCommandFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest, ILogger log)
        {
            StartFreeDriveControlCommandResponseModel startFreeDriveControlCommandResponseModel = new StartFreeDriveControlCommandResponseModel();

            string IOT_HUB_SERVICE_URL = Environment.GetEnvironmentVariable(IOT_HUB_SERVICE_URL_ENVIRONMENT_VARIABLE);
            if (IOT_HUB_SERVICE_URL is null)
            {
                return new BadRequestObjectResult(startFreeDriveControlCommandResponseModel.GetBadRequestRespondModel(IOT_HUB_SERVICE_URL_IS_NULL_MESSAGE));
            }
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                StartFreeDriveControlCommandResponseModel.CommandRequestModel startFreeDriveControlCommandRequestModel = JsonConvert
                    .DeserializeObject<StartFreeDriveControlCommandResponseModel.CommandRequestModel>(requestBody);
                if (startFreeDriveControlCommandRequestModel.DeviceId is null)
                {
                    return new BadRequestObjectResult(startFreeDriveControlCommandResponseModel
                        .GetBadRequestRespondModel(ERROR_NULL_VALUES_DETECTED_MESSAGE));
                }
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_SERVICE_URL);
                CloudToDeviceMethod cloudToDeviceMethod = new CloudToDeviceMethod(START_FREE_DRIVE_CONTROL_COMMAND_VARIABLE);
                cloudToDeviceMethod.ResponseTimeout = TimeSpan.FromSeconds(startFreeDriveControlCommandRequestModel.ResponseTimeout);
                CloudToDeviceMethodResult cloudToDeviceMethodResult = await serviceClient
                    .InvokeDeviceMethodAsync(startFreeDriveControlCommandRequestModel.DeviceId, cloudToDeviceMethod);
                startFreeDriveControlCommandResponseModel.CommandRequest = startFreeDriveControlCommandRequestModel;
                startFreeDriveControlCommandResponseModel.CommandResponse = StartFreeDriveControlCommandResponseModel.CommandResponseModel
                    .GetCommandResponseModel(cloudToDeviceMethodResult);
                if (startFreeDriveControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COMMAND_EXECUTION_SEQUENCE_ERROR_VARIABLE))
                {
                    return new BadRequestObjectResult(startFreeDriveControlCommandResponseModel
                        .GetBadRequestRespondModel(INCORRECT_SEQUENCE_COMMAND_CANNOT_RUN_MESSAGE));
                }
                if (startFreeDriveControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COBOT_CLIENT_EXECUTED_VARIABLE))
                {
                    return new OkObjectResult(startFreeDriveControlCommandResponseModel.GetOkRequestRespondModel(COMMAND_EXECUTED_SUCCESSFULLY_MESSAGE));
                }
            }
            catch (Exception exception)
            {
                ExceptionModel exceptionModel = ExceptionModel.GetFromException(exception);
                return new BadRequestObjectResult(startFreeDriveControlCommandResponseModel.GetBadRequestRespondModel(exceptionModel.Message));
            }
            return new BadRequestObjectResult(startFreeDriveControlCommandResponseModel.GetBadRequestRespondModel(SOMETHING_HAPPENED_MESSAGE));
        }
        [FunctionName("StopFreeDriveControlCommandFunction")]
        public static async Task<IActionResult> StopFreeDriveControlCommandFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest, ILogger log)
        {
            StopFreeDriveControlCommandResponseModel stopFreeDriveControlCommandResponseModel = new StopFreeDriveControlCommandResponseModel();

            string IOT_HUB_SERVICE_URL = Environment.GetEnvironmentVariable(IOT_HUB_SERVICE_URL_ENVIRONMENT_VARIABLE);
            if (IOT_HUB_SERVICE_URL is null)
            {
                return new BadRequestObjectResult(stopFreeDriveControlCommandResponseModel.GetBadRequestRespondModel(IOT_HUB_SERVICE_URL_IS_NULL_MESSAGE));
            }
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                StopFreeDriveControlCommandResponseModel.CommandRequestModel stopFreeDriveControlCommandRequestModel = JsonConvert
                    .DeserializeObject<StopFreeDriveControlCommandResponseModel.CommandRequestModel>(requestBody);
                if (stopFreeDriveControlCommandRequestModel.DeviceId is null)
                {
                    return new BadRequestObjectResult(stopFreeDriveControlCommandResponseModel
                        .GetBadRequestRespondModel(ERROR_NULL_VALUES_DETECTED_MESSAGE));
                }
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_SERVICE_URL);
                CloudToDeviceMethod cloudToDeviceMethod = new CloudToDeviceMethod(STOP_FREE_DRIVE_CONTROL_COMMAND_VARIABLE);
                cloudToDeviceMethod.ResponseTimeout = TimeSpan.FromSeconds(stopFreeDriveControlCommandRequestModel.ResponseTimeout);
                CloudToDeviceMethodResult cloudToDeviceMethodResult = await serviceClient
                    .InvokeDeviceMethodAsync(stopFreeDriveControlCommandRequestModel.DeviceId, cloudToDeviceMethod);
                stopFreeDriveControlCommandResponseModel.CommandRequest = stopFreeDriveControlCommandRequestModel;
                stopFreeDriveControlCommandResponseModel.CommandResponse = StopFreeDriveControlCommandResponseModel.CommandResponseModel
                    .GetCommandResponseModel(cloudToDeviceMethodResult);
                if (stopFreeDriveControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COMMAND_EXECUTION_SEQUENCE_ERROR_VARIABLE))
                {
                    return new BadRequestObjectResult(stopFreeDriveControlCommandResponseModel
                        .GetBadRequestRespondModel(INCORRECT_SEQUENCE_COMMAND_CANNOT_RUN_MESSAGE));
                }
                if (stopFreeDriveControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COBOT_CLIENT_EXECUTED_VARIABLE))
                {
                    return new OkObjectResult(stopFreeDriveControlCommandResponseModel.GetOkRequestRespondModel(COMMAND_EXECUTED_SUCCESSFULLY_MESSAGE));
                }
            }
            catch (Exception exception)
            {
                ExceptionModel exceptionModel = ExceptionModel.GetFromException(exception);
                return new BadRequestObjectResult(stopFreeDriveControlCommandResponseModel.GetBadRequestRespondModel(exceptionModel.Message));
            }
            return new BadRequestObjectResult(stopFreeDriveControlCommandResponseModel.GetBadRequestRespondModel(SOMETHING_HAPPENED_MESSAGE));
        }
        [FunctionName("PowerOnControlCommandFunction")]
        public static async Task<IActionResult> PowerOnControlCommandFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest, ILogger log)
        {
            PowerOnControlCommandResponseModel powerOnControlCommandResponseModel = new PowerOnControlCommandResponseModel();

            string IOT_HUB_SERVICE_URL = Environment.GetEnvironmentVariable(IOT_HUB_SERVICE_URL_ENVIRONMENT_VARIABLE);
            if (IOT_HUB_SERVICE_URL is null)
            {
                return new BadRequestObjectResult(powerOnControlCommandResponseModel.GetBadRequestRespondModel(IOT_HUB_SERVICE_URL_IS_NULL_MESSAGE));
            }
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                PowerOnControlCommandResponseModel.CommandRequestModel powerOnControlCommandRequestModel = JsonConvert
                    .DeserializeObject<PowerOnControlCommandResponseModel.CommandRequestModel>(requestBody);
                if (powerOnControlCommandRequestModel.DeviceId is null)
                {
                    return new BadRequestObjectResult(powerOnControlCommandResponseModel
                        .GetBadRequestRespondModel(ERROR_NULL_VALUES_DETECTED_MESSAGE));
                }
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_SERVICE_URL);
                CloudToDeviceMethod cloudToDeviceMethod = new CloudToDeviceMethod(POWER_ON_CONTROL_COMMAND_VARIABLE);
                cloudToDeviceMethod.ResponseTimeout = TimeSpan.FromSeconds(powerOnControlCommandRequestModel.ResponseTimeout);
                CloudToDeviceMethodResult cloudToDeviceMethodResult = await serviceClient
                    .InvokeDeviceMethodAsync(powerOnControlCommandRequestModel.DeviceId, cloudToDeviceMethod);
                powerOnControlCommandResponseModel.CommandRequest = powerOnControlCommandRequestModel;
                powerOnControlCommandResponseModel.CommandResponse = PowerOnControlCommandResponseModel.CommandResponseModel
                    .GetCommandResponseModel(cloudToDeviceMethodResult);
                if (powerOnControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COMMAND_EXECUTION_SEQUENCE_ERROR_VARIABLE))
                {
                    return new BadRequestObjectResult(powerOnControlCommandResponseModel
                        .GetBadRequestRespondModel(INCORRECT_SEQUENCE_COMMAND_CANNOT_RUN_MESSAGE));
                }
                if (powerOnControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COBOT_CLIENT_EXECUTED_VARIABLE))
                {
                    return new OkObjectResult(powerOnControlCommandResponseModel.GetOkRequestRespondModel(COMMAND_EXECUTED_SUCCESSFULLY_MESSAGE));
                }
            }
            catch (Exception exception)
            {
                ExceptionModel exceptionModel = ExceptionModel.GetFromException(exception);
                return new BadRequestObjectResult(powerOnControlCommandResponseModel.GetBadRequestRespondModel(exceptionModel.Message));
            }
            return new BadRequestObjectResult(powerOnControlCommandResponseModel.GetBadRequestRespondModel(SOMETHING_HAPPENED_MESSAGE));
        }
        [FunctionName("PowerOffControlCommandFunction")]
        public static async Task<IActionResult> PowerOffControlCommandFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest, ILogger log)
        {
            PowerOffControlCommandResponseModel powerOffControlCommandResponseModel = new PowerOffControlCommandResponseModel();

            string IOT_HUB_SERVICE_URL = Environment.GetEnvironmentVariable(IOT_HUB_SERVICE_URL_ENVIRONMENT_VARIABLE);
            if (IOT_HUB_SERVICE_URL is null)
            {
                return new BadRequestObjectResult(powerOffControlCommandResponseModel.GetBadRequestRespondModel(IOT_HUB_SERVICE_URL_IS_NULL_MESSAGE));
            }
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                PowerOffControlCommandResponseModel.CommandRequestModel powerOffControlCommandRequestModel = JsonConvert
                    .DeserializeObject<PowerOffControlCommandResponseModel.CommandRequestModel>(requestBody);
                if (powerOffControlCommandRequestModel.DeviceId is null)
                {
                    return new BadRequestObjectResult(powerOffControlCommandResponseModel
                        .GetBadRequestRespondModel(ERROR_NULL_VALUES_DETECTED_MESSAGE));
                }
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_SERVICE_URL);
                CloudToDeviceMethod cloudToDeviceMethod = new CloudToDeviceMethod(POWER_OFF_CONTROL_COMMAND_VARIABLE);
                cloudToDeviceMethod.ResponseTimeout = TimeSpan.FromSeconds(powerOffControlCommandRequestModel.ResponseTimeout);
                CloudToDeviceMethodResult cloudToDeviceMethodResult = await serviceClient
                    .InvokeDeviceMethodAsync(powerOffControlCommandRequestModel.DeviceId, cloudToDeviceMethod);
                powerOffControlCommandResponseModel.CommandRequest = powerOffControlCommandRequestModel;
                powerOffControlCommandResponseModel.CommandResponse = PowerOffControlCommandResponseModel.CommandResponseModel
                    .GetCommandResponseModel(cloudToDeviceMethodResult);
                if (powerOffControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COMMAND_EXECUTION_SEQUENCE_ERROR_VARIABLE))
                {
                    return new BadRequestObjectResult(powerOffControlCommandResponseModel
                        .GetBadRequestRespondModel(INCORRECT_SEQUENCE_COMMAND_CANNOT_RUN_MESSAGE));
                }
                if (powerOffControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COBOT_CLIENT_EXECUTED_VARIABLE))
                {
                    return new OkObjectResult(powerOffControlCommandResponseModel.GetOkRequestRespondModel(COMMAND_EXECUTED_SUCCESSFULLY_MESSAGE));
                }
            }
            catch (Exception exception)
            {
                ExceptionModel exceptionModel = ExceptionModel.GetFromException(exception);
                return new BadRequestObjectResult(powerOffControlCommandResponseModel.GetBadRequestRespondModel(exceptionModel.Message));
            }
            return new BadRequestObjectResult(powerOffControlCommandResponseModel.GetBadRequestRespondModel(SOMETHING_HAPPENED_MESSAGE));
        }
        [FunctionName("PlayControlCommandFunction")]
        public static async Task<IActionResult> PlayControlCommandFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest, ILogger log)
        {
            PlayControlCommandResponseModel playControlCommandResponseModel = new PlayControlCommandResponseModel();

            string IOT_HUB_SERVICE_URL = Environment.GetEnvironmentVariable(IOT_HUB_SERVICE_URL_ENVIRONMENT_VARIABLE);
            if (IOT_HUB_SERVICE_URL is null)
            {
                return new BadRequestObjectResult(playControlCommandResponseModel.GetBadRequestRespondModel(IOT_HUB_SERVICE_URL_IS_NULL_MESSAGE));
            }
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                PlayControlCommandResponseModel.CommandRequestModel playControlCommandRequestModel = JsonConvert
                    .DeserializeObject<PlayControlCommandResponseModel.CommandRequestModel>(requestBody);
                if (playControlCommandRequestModel.DeviceId is null)
                {
                    return new BadRequestObjectResult(playControlCommandResponseModel
                        .GetBadRequestRespondModel(ERROR_NULL_VALUES_DETECTED_MESSAGE));
                }
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_SERVICE_URL);
                CloudToDeviceMethod cloudToDeviceMethod = new CloudToDeviceMethod(PLAY_CONTROL_COMMAND_VARIABLE);
                cloudToDeviceMethod.ResponseTimeout = TimeSpan.FromSeconds(playControlCommandRequestModel.ResponseTimeout);
                CloudToDeviceMethodResult cloudToDeviceMethodResult = await serviceClient
                    .InvokeDeviceMethodAsync(playControlCommandRequestModel.DeviceId, cloudToDeviceMethod);
                playControlCommandResponseModel.CommandRequest = playControlCommandRequestModel;
                playControlCommandResponseModel.CommandResponse = PlayControlCommandResponseModel.CommandResponseModel
                    .GetCommandResponseModel(cloudToDeviceMethodResult);
                if (playControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COMMAND_EXECUTION_SEQUENCE_ERROR_VARIABLE))
                {
                    return new BadRequestObjectResult(playControlCommandResponseModel
                        .GetBadRequestRespondModel(INCORRECT_SEQUENCE_COMMAND_CANNOT_RUN_MESSAGE));
                }
                if (playControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COBOT_CLIENT_EXECUTED_VARIABLE))
                {
                    return new OkObjectResult(playControlCommandResponseModel.GetOkRequestRespondModel(COMMAND_EXECUTED_SUCCESSFULLY_MESSAGE));
                }
            }
            catch (Exception exception)
            {
                ExceptionModel exceptionModel = ExceptionModel.GetFromException(exception);
                return new BadRequestObjectResult(playControlCommandResponseModel.GetBadRequestRespondModel(exceptionModel.Message));
            }
            return new BadRequestObjectResult(playControlCommandResponseModel.GetBadRequestRespondModel(SOMETHING_HAPPENED_MESSAGE));
        }
        [FunctionName("PauseControlCommandFunction")]
        public static async Task<IActionResult> PauseControlCommandFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest, ILogger log)
        {
            PauseControlCommandResponseModel pauseControlCommandResponseModel = new PauseControlCommandResponseModel();

            string IOT_HUB_SERVICE_URL = Environment.GetEnvironmentVariable(IOT_HUB_SERVICE_URL_ENVIRONMENT_VARIABLE);
            if (IOT_HUB_SERVICE_URL is null)
            {
                return new BadRequestObjectResult(pauseControlCommandResponseModel.GetBadRequestRespondModel(IOT_HUB_SERVICE_URL_IS_NULL_MESSAGE));
            }
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                PauseControlCommandResponseModel.CommandRequestModel pauseControlCommandRequestModel = JsonConvert
                    .DeserializeObject<PauseControlCommandResponseModel.CommandRequestModel>(requestBody);
                if (pauseControlCommandRequestModel.DeviceId is null)
                {
                    return new BadRequestObjectResult(pauseControlCommandResponseModel
                        .GetBadRequestRespondModel(ERROR_NULL_VALUES_DETECTED_MESSAGE));
                }
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_SERVICE_URL);
                CloudToDeviceMethod cloudToDeviceMethod = new CloudToDeviceMethod(PAUSE_CONTROL_COMMAND_VARIABLE);
                cloudToDeviceMethod.ResponseTimeout = TimeSpan.FromSeconds(pauseControlCommandRequestModel.ResponseTimeout);
                CloudToDeviceMethodResult cloudToDeviceMethodResult = await serviceClient
                    .InvokeDeviceMethodAsync(pauseControlCommandRequestModel.DeviceId, cloudToDeviceMethod);
                pauseControlCommandResponseModel.CommandRequest = pauseControlCommandRequestModel;
                pauseControlCommandResponseModel.CommandResponse = PauseControlCommandResponseModel.CommandResponseModel
                    .GetCommandResponseModel(cloudToDeviceMethodResult);
                if (pauseControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COMMAND_EXECUTION_SEQUENCE_ERROR_VARIABLE))
                {
                    return new BadRequestObjectResult(pauseControlCommandResponseModel
                        .GetBadRequestRespondModel(INCORRECT_SEQUENCE_COMMAND_CANNOT_RUN_MESSAGE));
                }
                if (pauseControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COBOT_CLIENT_EXECUTED_VARIABLE))
                {
                    return new OkObjectResult(pauseControlCommandResponseModel.GetOkRequestRespondModel(COMMAND_EXECUTED_SUCCESSFULLY_MESSAGE));
                }
            }
            catch (Exception exception)
            {
                ExceptionModel exceptionModel = ExceptionModel.GetFromException(exception);
                return new BadRequestObjectResult(pauseControlCommandResponseModel.GetBadRequestRespondModel(exceptionModel.Message));
            }
            return new BadRequestObjectResult(pauseControlCommandResponseModel.GetBadRequestRespondModel(SOMETHING_HAPPENED_MESSAGE));
        }
        [FunctionName("CloseSafetyPopupControlCommandFunction")]
        public static async Task<IActionResult> CloseSafetyPopupControlCommandFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest, ILogger log)
        {
            CloseSafetyPopupControlCommandResponseModel closeSafetyPopupControlCommandResponseModel = 
                new CloseSafetyPopupControlCommandResponseModel();

            string IOT_HUB_SERVICE_URL = Environment.GetEnvironmentVariable(IOT_HUB_SERVICE_URL_ENVIRONMENT_VARIABLE);
            if (IOT_HUB_SERVICE_URL is null)
            {
                return new BadRequestObjectResult(closeSafetyPopupControlCommandResponseModel
                    .GetBadRequestRespondModel(IOT_HUB_SERVICE_URL_IS_NULL_MESSAGE));
            }
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                CloseSafetyPopupControlCommandResponseModel.CommandRequestModel closeSafetyPopupControlCommandRequestModel = JsonConvert
                    .DeserializeObject<CloseSafetyPopupControlCommandResponseModel.CommandRequestModel>(requestBody);
                if (closeSafetyPopupControlCommandRequestModel.DeviceId is null)
                {
                    return new BadRequestObjectResult(closeSafetyPopupControlCommandResponseModel
                        .GetBadRequestRespondModel(ERROR_NULL_VALUES_DETECTED_MESSAGE));
                }
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_SERVICE_URL);
                CloudToDeviceMethod cloudToDeviceMethod = new CloudToDeviceMethod(CLOSE_SAFETY_POPUP_CONTROL_COMMAND_VARIABLE);
                cloudToDeviceMethod.ResponseTimeout = TimeSpan.FromSeconds(closeSafetyPopupControlCommandRequestModel.ResponseTimeout);
                CloudToDeviceMethodResult cloudToDeviceMethodResult = await serviceClient
                    .InvokeDeviceMethodAsync(closeSafetyPopupControlCommandRequestModel.DeviceId, cloudToDeviceMethod);
                closeSafetyPopupControlCommandResponseModel.CommandRequest = closeSafetyPopupControlCommandRequestModel;
                closeSafetyPopupControlCommandResponseModel.CommandResponse = CloseSafetyPopupControlCommandResponseModel
                    .CommandResponseModel.GetCommandResponseModel(cloudToDeviceMethodResult);
                if (closeSafetyPopupControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COMMAND_EXECUTION_SEQUENCE_ERROR_VARIABLE))
                {
                    return new BadRequestObjectResult(closeSafetyPopupControlCommandResponseModel
                        .GetBadRequestRespondModel(INCORRECT_SEQUENCE_COMMAND_CANNOT_RUN_MESSAGE));
                }
                if (closeSafetyPopupControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COBOT_CLIENT_EXECUTED_VARIABLE))
                {
                    return new OkObjectResult(closeSafetyPopupControlCommandResponseModel.GetOkRequestRespondModel(COMMAND_EXECUTED_SUCCESSFULLY_MESSAGE));
                }
            }
            catch (Exception exception)
            {
                ExceptionModel exceptionModel = ExceptionModel.GetFromException(exception);
                return new BadRequestObjectResult(closeSafetyPopupControlCommandResponseModel.GetBadRequestRespondModel(exceptionModel.Message));
            }
            return new BadRequestObjectResult(closeSafetyPopupControlCommandResponseModel.GetBadRequestRespondModel(SOMETHING_HAPPENED_MESSAGE));
        }
        [FunctionName("UnlockProtectiveStopControlCommandFunction")]
        public static async Task<IActionResult> UnlockProtectiveStopControlCommandFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest, ILogger log)
        {
            UnlockProtectiveStopControlCommandResponseModel unlockProtectiveStopControlCommandResponseModel =
                new UnlockProtectiveStopControlCommandResponseModel();

            string IOT_HUB_SERVICE_URL = Environment.GetEnvironmentVariable(IOT_HUB_SERVICE_URL_ENVIRONMENT_VARIABLE);
            if (IOT_HUB_SERVICE_URL is null)
            {
                return new BadRequestObjectResult(unlockProtectiveStopControlCommandResponseModel
                    .GetBadRequestRespondModel(IOT_HUB_SERVICE_URL_IS_NULL_MESSAGE));
            }
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                UnlockProtectiveStopControlCommandResponseModel.CommandRequestModel unlockProtectiveStopControlCommandRequestModel = JsonConvert
                    .DeserializeObject<UnlockProtectiveStopControlCommandResponseModel.CommandRequestModel>(requestBody);
                if (unlockProtectiveStopControlCommandRequestModel.DeviceId is null)
                {
                    return new BadRequestObjectResult(unlockProtectiveStopControlCommandResponseModel
                        .GetBadRequestRespondModel(ERROR_NULL_VALUES_DETECTED_MESSAGE));
                }
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_SERVICE_URL);
                CloudToDeviceMethod cloudToDeviceMethod = new CloudToDeviceMethod(UNLOCK_PROTECTIVE_STOP_CONTROL_COMMAND_VARIABLE);
                cloudToDeviceMethod.ResponseTimeout = TimeSpan.FromSeconds(unlockProtectiveStopControlCommandRequestModel.ResponseTimeout);
                CloudToDeviceMethodResult cloudToDeviceMethodResult = await serviceClient
                    .InvokeDeviceMethodAsync(unlockProtectiveStopControlCommandRequestModel.DeviceId, cloudToDeviceMethod);
                unlockProtectiveStopControlCommandResponseModel.CommandRequest = unlockProtectiveStopControlCommandRequestModel;
                unlockProtectiveStopControlCommandResponseModel.CommandResponse = UnlockProtectiveStopControlCommandResponseModel
                    .CommandResponseModel.GetCommandResponseModel(cloudToDeviceMethodResult);
                if (unlockProtectiveStopControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COMMAND_EXECUTION_SEQUENCE_ERROR_VARIABLE))
                {
                    return new BadRequestObjectResult(unlockProtectiveStopControlCommandResponseModel
                        .GetBadRequestRespondModel(INCORRECT_SEQUENCE_COMMAND_CANNOT_RUN_MESSAGE));
                }
                if (unlockProtectiveStopControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COBOT_CLIENT_EXECUTED_VARIABLE))
                {
                    return new OkObjectResult(unlockProtectiveStopControlCommandResponseModel.GetOkRequestRespondModel(COMMAND_EXECUTED_SUCCESSFULLY_MESSAGE));
                }
            }
            catch (Exception exception)
            {
                ExceptionModel exceptionModel = ExceptionModel.GetFromException(exception);
                return new BadRequestObjectResult(unlockProtectiveStopControlCommandResponseModel.GetBadRequestRespondModel(exceptionModel.Message));
            }
            return new BadRequestObjectResult(unlockProtectiveStopControlCommandResponseModel.GetBadRequestRespondModel(SOMETHING_HAPPENED_MESSAGE));
        }
        [FunctionName("OpenPopupControlCommandFunction")]
        public static async Task<IActionResult> OpenPopupControlCommandFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest, ILogger log)
        {
            OpenPopupControlCommandResponseModel openPopupControlCommandResponseModel =
                new OpenPopupControlCommandResponseModel();

            string IOT_HUB_SERVICE_URL = Environment.GetEnvironmentVariable(IOT_HUB_SERVICE_URL_ENVIRONMENT_VARIABLE);
            if (IOT_HUB_SERVICE_URL is null)
            {
                return new BadRequestObjectResult(openPopupControlCommandResponseModel
                    .GetBadRequestRespondModel(IOT_HUB_SERVICE_URL_IS_NULL_MESSAGE));
            }
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                OpenPopupControlCommandResponseModel.CommandRequestModel openPopupControlCommandRequestModel = JsonConvert
                    .DeserializeObject<OpenPopupControlCommandResponseModel.CommandRequestModel>(requestBody);
                if (openPopupControlCommandRequestModel.DeviceId is null)
                {
                    return new BadRequestObjectResult(openPopupControlCommandResponseModel
                        .GetBadRequestRespondModel(ERROR_NULL_VALUES_DETECTED_MESSAGE));
                }
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_SERVICE_URL);
                CloudToDeviceMethod cloudToDeviceMethod = new CloudToDeviceMethod(OPEN_POPUP_CONTROL_COMMAND_VARIABLE);
                cloudToDeviceMethod.ResponseTimeout = TimeSpan.FromSeconds(openPopupControlCommandRequestModel.ResponseTimeout);
                cloudToDeviceMethod.SetPayloadJson(JsonConvert.SerializeObject(openPopupControlCommandRequestModel.Payload));
                CloudToDeviceMethodResult cloudToDeviceMethodResult = await serviceClient
                    .InvokeDeviceMethodAsync(openPopupControlCommandRequestModel.DeviceId, cloudToDeviceMethod);
                openPopupControlCommandResponseModel.CommandRequest = openPopupControlCommandRequestModel;
                openPopupControlCommandResponseModel.CommandResponse = OpenPopupControlCommandResponseModel
                    .CommandResponseModel.GetCommandResponseModel(cloudToDeviceMethodResult);
                if (openPopupControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COMMAND_EXECUTION_SEQUENCE_ERROR_VARIABLE))
                {
                    return new BadRequestObjectResult(openPopupControlCommandResponseModel
                        .GetBadRequestRespondModel(INCORRECT_SEQUENCE_COMMAND_CANNOT_RUN_MESSAGE));
                }
                if (openPopupControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COBOT_CLIENT_EXECUTED_VARIABLE))
                {
                    return new OkObjectResult(openPopupControlCommandResponseModel.GetOkRequestRespondModel(COMMAND_EXECUTED_SUCCESSFULLY_MESSAGE));
                }
            }
            catch (Exception exception)
            {
                ExceptionModel exceptionModel = ExceptionModel.GetFromException(exception);
                return new BadRequestObjectResult(openPopupControlCommandResponseModel.GetBadRequestRespondModel(exceptionModel.Message));
            }
            return new BadRequestObjectResult(openPopupControlCommandResponseModel.GetBadRequestRespondModel(SOMETHING_HAPPENED_MESSAGE));
        }
        [FunctionName("ClosePopupControlCommandFunction")]
        public static async Task<IActionResult> ClosePopupControlCommandFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest, ILogger log)
        {
            ClosePopupControlCommandResponseModel closePopupControlCommandResponseModel =
                new ClosePopupControlCommandResponseModel();

            string IOT_HUB_SERVICE_URL = Environment.GetEnvironmentVariable(IOT_HUB_SERVICE_URL_ENVIRONMENT_VARIABLE);
            if (IOT_HUB_SERVICE_URL is null)
            {
                return new BadRequestObjectResult(closePopupControlCommandResponseModel
                    .GetBadRequestRespondModel(IOT_HUB_SERVICE_URL_IS_NULL_MESSAGE));
            }
            try
            {
                string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                ClosePopupControlCommandResponseModel.CommandRequestModel closePopupControlCommandRequestModel = JsonConvert
                    .DeserializeObject<ClosePopupControlCommandResponseModel.CommandRequestModel>(requestBody);
                if (closePopupControlCommandRequestModel.DeviceId is null)
                {
                    return new BadRequestObjectResult(closePopupControlCommandResponseModel
                        .GetBadRequestRespondModel(ERROR_NULL_VALUES_DETECTED_MESSAGE));
                }
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_SERVICE_URL);
                CloudToDeviceMethod cloudToDeviceMethod = new CloudToDeviceMethod(CLOSE_POPUP_CONTROL_COMMAND_VARIABLE);
                cloudToDeviceMethod.ResponseTimeout = TimeSpan.FromSeconds(closePopupControlCommandRequestModel.ResponseTimeout);
                CloudToDeviceMethodResult cloudToDeviceMethodResult = await serviceClient
                    .InvokeDeviceMethodAsync(closePopupControlCommandRequestModel.DeviceId, cloudToDeviceMethod);
                closePopupControlCommandResponseModel.CommandRequest = closePopupControlCommandRequestModel;
                closePopupControlCommandResponseModel.CommandResponse = ClosePopupControlCommandResponseModel
                    .CommandResponseModel.GetCommandResponseModel(cloudToDeviceMethodResult);
                if (closePopupControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COMMAND_EXECUTION_SEQUENCE_ERROR_VARIABLE))
                {
                    return new BadRequestObjectResult(closePopupControlCommandResponseModel
                        .GetBadRequestRespondModel(INCORRECT_SEQUENCE_COMMAND_CANNOT_RUN_MESSAGE));
                }
                if (closePopupControlCommandResponseModel.CommandResponse.Payload.Status.Equals(COBOT_CLIENT_EXECUTED_VARIABLE))
                {
                    return new OkObjectResult(closePopupControlCommandResponseModel.GetOkRequestRespondModel(COMMAND_EXECUTED_SUCCESSFULLY_MESSAGE));
                }
            }
            catch (Exception exception)
            {
                ExceptionModel exceptionModel = ExceptionModel.GetFromException(exception);
                return new BadRequestObjectResult(closePopupControlCommandResponseModel.GetBadRequestRespondModel(exceptionModel.Message));
            }
            return new BadRequestObjectResult(closePopupControlCommandResponseModel.GetBadRequestRespondModel(SOMETHING_HAPPENED_MESSAGE));
        }
    }
}