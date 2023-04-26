using UnityEngine;
using System.Globalization;

[System.Serializable]
public class ClientFeatures
{
    public string client_id = "null";
    public string install_referrer = "null";
    public string carrier_code = "null";
    public string app_device_type = "null";
    public string app_locale = "null";
    public string app_device_name = "null";
    public string app_client_id = "null";
    public string app_advertising_id = "null";
    public string package = "null";

    public ClientFeatures()
    {
        GetAndroidAdvertiserId();

        client_id = SystemInfo.deviceUniqueIdentifier;
        install_referrer = Application.installerName;
        carrier_code = CultureInfo.CurrentCulture.Parent.ToString();
        app_device_type = SystemInfo.deviceType.ToString();
        app_locale = CultureInfo.CurrentCulture.Name;
        app_device_name = SystemInfo.deviceName;
        app_client_id = SystemInfo.deviceUniqueIdentifier;

        package = Application.identifier;
    }

    private void GetAndroidAdvertiserId()
    {
        MiniIT.Utils.AdvertisingIdFetcher.RequestAdvertisingId(advertisingId =>
        {
            app_advertising_id = advertisingId;
        });
    }
}
