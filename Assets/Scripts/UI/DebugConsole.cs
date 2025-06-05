using UnityEngine;
using TMPro;

public class DebugConsole : MonoBehaviour
{
    public TMP_Text debugText;
    private string logBuffer = "";

    //void Start()
    //{
    //    debugText.text = "Test <color=green>Green</color>!";
    //}

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        logBuffer += logString + "\n";
        if (logBuffer.Length > 5000) // Optional: limit text length
            logBuffer = logBuffer.Substring(logBuffer.Length - 5000);

        debugText.text = logBuffer;
    }
}
