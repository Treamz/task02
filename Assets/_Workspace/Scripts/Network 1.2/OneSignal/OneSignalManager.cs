using UnityEngine;
using OneSignalSDK;
using Newtonsoft.Json;
using QuintaEssenta.Library.DI;

public class OneSignalManager : BaseBehaviour
{
    public string FileName { get; private set; } = "/oneSignalResponse.json";
    public string Path { get; private set; } = "/_Workspace/Response";

    private string _additionalData;

    private void _notificationOpened(NotificationOpenedResult result)
    {
        SerializeManager serializeManager = new SerializeManager(FileName, Path);

        OneSignalResult data = JsonConvert.DeserializeObject<OneSignalResult>(JsonUtility.ToJson(result));

        RawPayLoad rawPayLoad = JsonConvert.DeserializeObject<RawPayLoad>(data.notification.rawPayload);

        string custom = serializeManager.AdditionalDataToJsonFormat(rawPayLoad.custom);
        CustomData customData = JsonConvert.DeserializeObject<CustomData>(custom);

        _additionalData = customData.a;

        SerializeManager serializeManagerAdditionalData = new SerializeManager("/additionalData.json", Path);
        serializeManagerAdditionalData.SaveData(customData.a);

        CustomDebug.LogFile($"Notification opened");
    }

    public string GetAdditinalData()
    {
        SerializeManager serializeManagerAdditionalData = new SerializeManager("/additionalData.json", Path);

        _additionalData = serializeManagerAdditionalData.LoadStringFormat();

        if (string.IsNullOrEmpty(_additionalData) == true)
            _additionalData = "null";

        CustomDebug.LogFile($"AdditionalData: {_additionalData}");

        return _additionalData;
    }

    public void InitOneSignal(ServerFeatures data)
    {
        if (!string.IsNullOrEmpty(data.onesignal.push_id))
        {
            OneSignal.Default.Initialize(data.onesignal.push_id);
            OneSignal.Default.NotificationOpened += _notificationOpened;

            CustomDebug.LogFile($"Onesingal initialize with: {data.onesignal.push_id}");
        }
        else
        {
            CustomDebug.LogFile($"Onesingal push id is empty: {data.onesignal.push_id}");
        }
    }

    public void SetTag(DeeplinkFeatures data)
    {
        OneSignal.Default.SendTag($"{data.partnerName}", data.partnerName);

        CustomDebug.LogFile($"Onesingal set tag with: {data.partnerName}");
    }
}
