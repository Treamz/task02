using UnityEngine;
using System.IO;

public class SerializeManager
{
    private string _fileName;
    private string _editorDirectory;
    private string _path;

    public SerializeManager(string fileName, string editorDirectory)
    {
        _fileName = fileName;
        _editorDirectory = editorDirectory;

#if UNITY_ANDROID && !UNITY_EDITOR
        _path = $"{Application.persistentDataPath}{Path.AltDirectorySeparatorChar}";
#else
        _path = $"{Application.dataPath}{_editorDirectory}";
#endif
    }

    public bool DataAvailable() =>
        File.Exists(_path + _fileName);

    public void SaveData(string text)
    {
        if (Directory.Exists(_path) == false)
            Directory.CreateDirectory(_path);

        using (var writer = new StreamWriter(_path + _fileName))
        {
            writer.WriteLine(text);
        }
    }

    public string LoadStringFormat()
    {
        string value = "";

        if (!File.Exists(_path + _fileName))
        {
            Debug.Log(_path + _fileName + " not found.");

            return value;
        }

        using (var reader = new StreamReader(_path + _fileName))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                value += line;
            }

            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
        }

        return value;
    }

    public T LoadData<T>(T value)
    {
        if (!File.Exists(_path + _fileName))
            return value;

        string json = "";

        using (var reader = new StreamReader(_path + _fileName))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                json += line;
            }

            if (string.IsNullOrEmpty(json))
            {
                return value;
            }

            return JsonUtility.FromJson<T>(json);
        }
    }

    public string DeepLinkToJsonFormat(string value)
    {
        string newValue = "";
        bool isStart = false;

        for (int i = 0; i < value.Length; i++)
        {
            if (isStart == true)
            {
                if (value[i] == '=')
                {
                    newValue += "\":\"";
                }
                else if (value[i] == '$' || value[i] == '{' || value[i] == '}')
                {

                }
                else if (value[i] == '&')
                {
                    newValue += "\",\"";
                }
                else if (value[i] == ' ')
                {

                }
                else
                {
                    newValue += value[i];
                }
            }

            if (value[i] == '?')
            {
                isStart = true;

                newValue += "{\"";
            }
        }

        newValue += "\"}";

        return newValue;
    }

    //[af_status, Organic][campaign, watrusha][campaign_id, 1234556][af_channel, chacha][network, pubmae]
    //{"af_status":"Organic","campaign":"watrusha","campaign_id":"1234556","af_channel":"chacha","network":"pubmae"}

    public string ConversionDataToJsonFormat(string value)
    {
        string newValue = "{";

        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] == '[')
            {
                newValue += "\"";
            }
            else if (value[i] == ',')
            {
                newValue += "\":\"";
            }
            else if (value[i] == ']')
            {
                if (i == value.Length - 1) { }
                else
                {
                    newValue += "\",";
                }
            }
            else
            {
                newValue += value[i];
            }
        }

        newValue += "\"}";

        return newValue;
    }

    public string AdditionalDataToJsonFormat(string value)
    {
        string newValue = "{";

        bool isString = false;

        for (int i = 1; i < value.Length-1; i++)
        {
            if (value[i] == '{')
            {
                newValue += '"';
                newValue += value[i];
            }
            else if (value[i] == '}')
            {
                newValue += value[i];
                newValue += '"';

                isString = true;
            }
            else if (value[i] == '"')
            {
                if (i > 5 && isString == false)
                {
                    newValue += '\\';

                    newValue += value[i];
                }
                else
                {
                    newValue += value[i];
                }
            }
            else
            {
                newValue += value[i];
            }
        }

        newValue += '}';

        return newValue;
    }

    //public static string InstallReferrerToJson(string value)
    //{
    //    string newValue = "{\"";

    //    for (int i = 0; i < value.Length; i++)
    //    {
    //        newValue += value[i];
    //    }

    //    return newValue;
    //}

    public T LoadDataBase<T>(T value)
    {
        if (!File.Exists(_path + _fileName))
            return value;

        string json = "";

        using (var reader = new StreamReader(_path + _fileName))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                json += line;
            }

            if (string.IsNullOrEmpty(json))
            {
                return value;
            }

            return JsonUtility.FromJson<T>(json);
        }
    }
}
