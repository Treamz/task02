using UnityEngine;
using QuintaEssenta.Library.DI;
using System.Collections;

[RequireComponent(typeof(ServerRequest))]
public class NetworkManager : BaseBehaviour
{
    [SerializeField]
    private string _domain;

    private UniWebView _webview;

    [Inject]
    private OneSignalManager _onesignal;

    [Inject]
    private FacebookManager _facebook;

    [Inject]
    private AppsFlyerManager _appsflyer;

    [Inject]
    private LevelManager _levelManager;

    [Inject]
    private ServerRequest _serverRequset;

    protected override void Awake()
    {
        base.Awake();
        _webview = FindObjectOfType<UniWebView>();
    }

    private void OnEnable()
    {
        _serverRequset.OnWebRequestCompleted += delegate ()
        {
            Network();
        };
    }

    private void Start()
    {
        StartCoroutine(Pause());
    }

    private IEnumerator Pause()
    {
        yield return new WaitForSeconds(1f);
        _serverRequset.PostRequest(_domain);
    }

    private void Network()
    {
        Debug.Log($"Response: {_serverRequset.GetWebRequest()}");

        ServerFeatures data = new ServerFeatures();
        SerializeManager response = new SerializeManager("/response.json", "/_Workspace/Response");

        if (_serverRequset.GetWebRequest() == null)
        {
            OpenPlug();
        }
        else if (response.DataAvailable() == true)
        {
            try { data = response.LoadData(new ServerFeatures()); }
            catch 
            {
                response.SaveData(_serverRequset.GetWebRequest());

                try { data = response.LoadData(new ServerFeatures()); }
                catch { data = new ServerFeatures(); }
            }

            if (data.error == true)
            {
                response.SaveData(_serverRequset.GetWebRequest());

                try { data = response.LoadData(new ServerFeatures()); }
                catch { data = new ServerFeatures(); }
            }
        }
        else if (response.DataAvailable() == false)
        {
            response.SaveData(_serverRequset.GetWebRequest());

            try { data = response.LoadData(new ServerFeatures()); }
            catch { data = new ServerFeatures(); }
        }

        if (string.IsNullOrEmpty(data.webview))
            OpenPlug();
        else 
            OpenWebview(data);
    }

    private void OpenWebview(ServerFeatures data)
    {
        Screen.orientation = ScreenOrientation.AutoRotation;

        DeeplinkFeatures dataDeepLink = new DeeplinkFeatures();

        if (data.facebook.isEnabled == true)
        {
            _facebook.OnDeepLinkComplete += delegate ()
            {
                FaceBookInitComplete(dataDeepLink, data);
            };

            string appId = data.facebook.fbid;
            _facebook.Init(appId);
        }
        else
        {
            FaceBookInitComplete(dataDeepLink, data);
        }
    }

    private void FaceBookInitComplete(DeeplinkFeatures dataDeepLink, ServerFeatures data)
    {
        if (_facebook.IsDeepLinkIn() == false && data.appsflyer.isEnabled)
        {
            _appsflyer.OnConversionDataSuccess += delegate ()
            {
                dataDeepLink = _appsflyer.GetDeeplink();

                CustomDebug.LogFile($"Appsflyer deeplink: {JsonUtility.ToJson(dataDeepLink)}");

                _onesignal.InitOneSignal(data);
                _onesignal.SetTag(dataDeepLink);

                StartCoroutine(StartWebview(data, dataDeepLink));
            };

            _appsflyer.devKey = data.appsflyer.app_token;
            _appsflyer.Init();
        }
        else if (_facebook.IsDeepLinkIn() == true)
        {
            dataDeepLink = _facebook.GetDeepLink();

            CustomDebug.LogFile($"Facebook deeplink: {JsonUtility.ToJson(dataDeepLink)}");

            _onesignal.InitOneSignal(data);
            _onesignal.SetTag(dataDeepLink);

            StartCoroutine(StartWebview(data, dataDeepLink));
        }
        else
        {
            StartCoroutine(StartWebview(data, dataDeepLink));
        }
    }

    private IEnumerator StartWebview(ServerFeatures data, DeeplinkFeatures dataDeepLink)
    {
        yield return new WaitForSeconds(1f);

        string webview = Webview(data.webview, dataDeepLink, data);
        CustomDebug.LogFile($"Webview show: {webview}");
        _webview.Show();
        _webview.Load(webview);
    }

    private string Webview(string webview, DeeplinkFeatures deeplinkFeatures, ServerFeatures data)
    {
        string newWebview = "";

        for (int i = 0; i < webview.Length; i++)
        {
            if (webview[i] == '?')
            {
                newWebview += webview[i];
                break;
            }
            else
            {
                newWebview += webview[i];
            }
        }

        string parameters =
            $"external_id={_serverRequset.GetClient().app_client_id}&" +
            $"creative_id={_appsflyer.ConversionFeatures.af_channel}&" +
            $"ad_campaign_id={_appsflyer.ConversionFeatures.campaign_id}&" +
            $"source={_appsflyer.ConversionFeatures.network}&" +
            $"sub_id_1={deeplinkFeatures.sub1}&" +
            $"sub_id_2={deeplinkFeatures.sub2}&" +
            $"sub_id_3={deeplinkFeatures.sub3}&" +
            $"sub_id_4={deeplinkFeatures.sub4}&" +
            $"sub_id_5={deeplinkFeatures.sub5}&" +
            $"stream_id={deeplinkFeatures.partnerStream}&" +
            $"campaign_name={deeplinkFeatures.partnerName}&" +
            $"advertising_id={_serverRequset.GetClient().app_advertising_id}&" +
            $"adid={_appsflyer.GetUID()}&" +
            $"install_referrer={UnityEngine.Networking.UnityWebRequest.EscapeURL(_serverRequset.GetInstallRefferer())}&" +
            $"push_data={_onesignal.GetAdditinalData()}";

        newWebview += parameters;

        CustomDebug.LogFile(UnityEngine.Networking.UnityWebRequest.EscapeURL(_serverRequset.GetInstallRefferer()));

        return newWebview;
    }

    private void OpenPlug()
    {
        Screen.orientation = ScreenOrientation.Portrait;

        _levelManager.ShowPlug();
    }
}
