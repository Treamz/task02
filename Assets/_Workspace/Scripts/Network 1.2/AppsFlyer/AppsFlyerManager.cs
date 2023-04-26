using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;
using QuintaEssenta.Library.DI;

// This class is intended to be used the the AppsFlyerObject.prefab
public class AppsFlyerManager : BaseBehaviour, IAppsFlyerConversionData
{
    public string FileNameData { get; private set; } = "/AppsflyerDeeplink.json";
    public string FileName { get; private set; } = "/conversionData.json";
    public string Path { get; private set; } = "/_Workspace/Response";

    // These fields are set from the editor so do not modify!
    //******************************//
    public string devKey;
    public string appID;
    public string UWPAppID;
    public string macOSAppID;
    public bool isDebug;
    public bool getConversionData;
    //******************************//

    public ConversionFeatures ConversionFeatures;

    private DeeplinkFeatures _deeplinkFeatures;

    private string _conversionData;

    public delegate void ConversionDataSuccessHandler();
    public event ConversionDataSuccessHandler OnConversionDataSuccess;

    protected override void Awake()
    {
        base.Awake();

        ConversionFeatures = new ConversionFeatures();
    }

    public void Init()
    {
        AppsFlyer.setIsDebug(isDebug);
        try
        {
#if UNITY_WSA_10_0 && !UNITY_EDITOR
        AppsFlyer.initSDK(devKey, UWPAppID, getConversionData ? this : null);
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
        AppsFlyer.initSDK(devKey, macOSAppID, getConversionData ? this : null);
#else
            AppsFlyer.initSDK(devKey, appID, getConversionData ? this : null);
#endif

            AppsFlyer.startSDK();

            CustomDebug.LogFile("Appsflyer init");
        }
        catch 
        {
            CustomDebug.LogFile("Appsflyer error");

            OnConversionDataSuccess?.Invoke();
        }
    }

    public string GetUID()
    {
        string value = AppsFlyer.getAppsFlyerId();
        
        if (string.IsNullOrEmpty(value) == true)
        {
            value = "null";
        }

        return value;
    }

    // Mark AppsFlyer CallBacks
    public void onConversionDataSuccess(string conversionData)
    {
        AppsFlyer.AFLog("didReceiveConversionData", conversionData);
        Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);

        _conversionData = "";

        foreach (var item in conversionDataDictionary)
        {
            _conversionData += item;
        }

        ConversionFeatures = GetConversionData();

        OnConversionDataSuccess?.Invoke();

        // add deferred deeplink logic here
    }

    public void onConversionDataFail(string error)
    {
        AppsFlyer.AFLog("didReceiveConversionDataWithError", error);

        CustomDebug.LogFile($"didReceiveConversionDataWithError {error}");

        OnConversionDataSuccess?.Invoke();
    }

    public void onAppOpenAttribution(string attributionData)
    {
        AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
        Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
        // add direct deeplink logic here

        CustomDebug.LogFile($"onAppOpenAttribution {attributionData}");
    }

    public void onAppOpenAttributionFailure(string error)
    {
        AppsFlyer.AFLog("onAppOpenAttributionFailure", error);

        CustomDebug.LogFile($"onAppOpenAttributionFailure {error}");
    }

    public DeeplinkFeatures GetDeeplink()
    {
        if (_deeplinkFeatures == null)
        {
            try
            {
                SerializeManager serializeManagerDeepLink = new SerializeManager(FileNameData, Path);
                _deeplinkFeatures = serializeManagerDeepLink.LoadData(new DeeplinkFeatures());
            }
            catch (System.Exception)
            {
                _deeplinkFeatures = new DeeplinkFeatures();
            }
        }

        return _deeplinkFeatures;
    }

    private ConversionFeatures GetConversionData()
    {
        string data = "";

        SerializeManager serializeManager = new SerializeManager(FileName, Path);

#if UNITY_ANDROID && !UNITY_EDITOR
        data = _conversionData;
#else
        data = "[af_status, Organic][campaign, partnerName=${partnerName}&partnerStream=${partnerStream}&sub1=${sub1}&sub2=${sub2}&sub3=${sub3}&sub4=${sub4}&sub5=${sub5}][campaign_id, 1234556][af_channel, chacha][network, pubmae]";
#endif

        ConversionFeatures conversionFeatures = new ConversionFeatures();

        CustomDebug.LogFile($"Conversion data: {data}");

        if (string.IsNullOrEmpty(data) == false)
        {
            data = serializeManager.ConversionDataToJsonFormat(data);
            serializeManager.SaveData(data);
            conversionFeatures = serializeManager.LoadData(new ConversionFeatures());
        }

        if (string.IsNullOrEmpty(conversionFeatures.campaign) == true || conversionFeatures.campaign == "null")
        {

        }
        else
        {
            string deepLinkData = "?" + conversionFeatures.campaign;
            deepLinkData = serializeManager.DeepLinkToJsonFormat(deepLinkData);

            SerializeManager serializeManagerDeepLink = new SerializeManager(FileNameData, Path);
            serializeManagerDeepLink.SaveData(deepLinkData);

            CustomDebug.LogFile("Save deepLink data");
        }

        return conversionFeatures;
    }
}
