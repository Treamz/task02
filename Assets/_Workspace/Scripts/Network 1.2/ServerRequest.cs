using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Ugi.PlayInstallReferrerPlugin;
using QuintaEssenta.Library.DI;

public class ServerRequest : BaseBehaviour
{
    public delegate void WebRequestCompletedHandler();
    public WebRequestCompletedHandler OnWebRequestCompleted;

    private UnityWebRequest _webRequest;
    private ClientFeatures _clientFeatures;

    private delegate void InstallReferrerHandler();
    private event InstallReferrerHandler OnInstallReferrer;

    private string _install_referrer = "null";

    protected override void Awake()
    {
        base.Awake();

        _clientFeatures = new ClientFeatures();
    }

    public ClientFeatures GetClient() =>
        _clientFeatures;

    public string GetInstallRefferer() =>
        _install_referrer;

    public string GetWebRequest()
    {
        if (_webRequest == null)
            return null;

        return Base64Convertor.ReturnFromBase64(_webRequest.downloadHandler.text);
    }

    public void PostRequest(string url)
    {
        OnInstallReferrer += delegate ()
        {
            StartCoroutine(RequestRoutine(url));
        };

        PlayInstallReferrer.GetInstallReferrerInfo((installReferrerDetails) => //Error
        {
            if (installReferrerDetails.InstallReferrer != null)
            {
                _install_referrer = installReferrerDetails.InstallReferrer;
            }

            CustomDebug.LogFile($"Install referrer: {_install_referrer}");

            OnInstallReferrer?.Invoke();
        });
    }

    private IEnumerator RequestRoutine(string url)
    {
        var clientJson = JsonUtility.ToJson(_clientFeatures);
        var clientBase64 = Base64Convertor.ReturnInBase64(clientJson);

        var uwr = new UnityWebRequest(url + clientBase64, "POST");
        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.SetRequestHeader("X-Requested-With", _clientFeatures.package);

        CustomDebug.LogFile($"Client: {clientJson}");

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            _webRequest = null;

            CustomDebug.LogFile("Connectiond Error");
        }
        else
        {
            _webRequest = uwr;

            CustomDebug.LogFile("Connectiond Success");
        }

        OnWebRequestCompleted?.Invoke();

        uwr.Dispose();
    }
}
