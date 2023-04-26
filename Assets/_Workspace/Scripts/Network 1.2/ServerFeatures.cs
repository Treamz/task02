[System.Serializable]
public class ServerFeatures : SerializableData
{
    public bool error;
    public string name;
    public string webview;
    public Appsflyer appsflyer;
    public Onesignal onesignal;
    public FaceBook facebook;
}

[System.Serializable]
public class Appsflyer
{
    public string app_token;
    public bool isEnabled;
}

[System.Serializable]
public class Onesignal
{
    public string push_id;
    public bool isEnabled;
}

[System.Serializable]
public class FaceBook
{
    public bool isEnabled;
    public string fbid = "null";
    public string clientToken;
}
