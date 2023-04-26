using UnityEngine;

public static class CustomDebug
{
    public delegate void UpdateDebugHandler<T>(T obj);
    public static event UpdateDebugHandler<string> OnUpdateDebug;

    public static string DebugText { get; private set; }

    public static void LogFile(string value)
    {
        DebugText += $" - {value}\n";
        Debug.Log(DebugText);

        OnUpdateDebug?.Invoke(DebugText);
    }
}
