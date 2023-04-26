[System.Serializable]
public class OneSignalResult : SerializableData
{
    public Notification notification;
}

[System.Serializable]
public class Notification : SerializableData
{
    public string rawPayload;
}

[System.Serializable]
public class RawPayLoad : SerializableData
{
    public string custom;
}

[System.Serializable]
public class CustomData : SerializableData
{
    public string a;
}
