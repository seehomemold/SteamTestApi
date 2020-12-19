using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShowLogMessage : MonoBehaviour
{
    //public string output = "";
    public string stack = "";
    public int maxShowLine = 9;
    private string[] output;
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        output = new string[maxShowLine];
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = "";
        }
        Application.logMessageReceived += OnLogCallBack;
        text = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        string show = "";
        for (int i = 0; i < output.Length; i++) {
            show += output[i] + "\n";
        }
        text.text = show;
    }

    void OnLogCallBack(string logString, string stackTrace, LogType type)
    {
        for(int i = 1; i < output.Length; i++)
        {
            output[i-1] = output[i];
        }
        output[output.Length-1] = logString;
        stack = stackTrace;
    }

}
