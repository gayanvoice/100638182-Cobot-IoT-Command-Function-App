namespace CobotIoTCommandFunctionApp.Model
{
    public class IotCommandModel
    {
        public string DeviceId { get; set; }
        public string Command { get; set; }
        public double ResponseTimeout { get; set; } = 20.0;
    }
}