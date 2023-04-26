using UnityEngine;
using TMPro;

public class DebugPanel : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        CustomDebug.OnUpdateDebug += delegate (string value)
        {
            UpdateText(value);
        };
    }

    private void OnDisable()
    {
        CustomDebug.OnUpdateDebug -= delegate (string value)
        {
            UpdateText(value);
        };
    }

    private void UpdateText(string text)
    {
        _text.text = text;
    }
}
