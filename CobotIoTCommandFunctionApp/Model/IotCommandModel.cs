using Newtonsoft.Json;

namespace CobotIoTCommandFunctionApp.Model
{
    public class IoTCommandModel
    {
        [JsonProperty("iot_device_id")]
        public string IoTDeviceId { get; set; }
        [JsonProperty("iot_command")]
        public string IoTCommand { get; set; }
        [JsonProperty("iot_payload")]
        public string IoTPayload { get; set; } = "iot_payload";
    }
}