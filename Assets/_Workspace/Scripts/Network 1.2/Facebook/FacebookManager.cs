using UnityEngine;
using Facebook.Unity;
using System;
using QuintaEssenta.Library.DI;

public class FacebookManager : BaseBehaviour
{
    public string FileName { get; private set; } = "/faceBookDeeplink.json";
    public string Path { get; private set; } = "/_Workspace/Response";

    public delegate void InitCompleteHandler();
    public InitCompleteHandler OnInitComplete;
    public InitCompleteHandler OnDeepLinkComplete;

    private DeeplinkFeatures _deepLinkFeatures = new DeeplinkFeatures();

    public void Init(string appId)
    {
        FB.Init(appId, onInitComplete: () =>
        {
            FB.ActivateApp();
            FB.Mobile.FetchDeferredAppLinkData(DeepLinkCallback);

            CustomDebug.LogFile("Facebook init complete");

            OnInitComplete?.Invoke();
        });
    }

    private void DeepLinkCallback(IAppLinkResult result)
    {
        if (!String.IsNullOrEmpty(result.TargetUrl))
        {
            SerializeManager deeplink = new SerializeManager(FileName, Path);

            string datalink;

#if UNITY_ANDROID && !UNITY_EDITOR
            datalink = result.TargetUrl;
#else
            datalink = "app://?partnerName=${partnerName}&partnerStream=${partnerStream}&sub1=${sub1}&sub2=${sub2}&sub3=${sub3}&sub4=${sub4}&sub5=${sub5}";
#endif
            datalink = deeplink.DeepLinkToJsonFormat(datalink);
            _deepLinkFeatures = JsonUtility.FromJson<DeeplinkFeatures>(datalink);
            deeplink.SaveData(datalink);
        }

        OnDeepLinkComplete?.Invoke();
    }

    public bool IsDeepLinkIn()
    {
        string value = GetDeepLink().partnerName;

        if (string.IsNullOrEmpty(value) == true || value == "null")
            return false;
        else return true;
    }

    public DeeplinkFeatures GetDeepLink()
    {
        try
        {
            SerializeManager deeplink = new SerializeManager(FileName, Path);
            _deepLinkFeatures = deeplink.LoadData(new DeeplinkFeatures());
        }
        catch
        {
            _deepLinkFeatures = new DeeplinkFeatures();
        }


        return _deepLinkFeatures;
    }
}
