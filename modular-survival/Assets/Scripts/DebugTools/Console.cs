using UnityEngine;
using TMPro;
using System;

public class Console : MonoBehaviour
{
    private static TMP_Text textArea;

    void Start()
    {
        textArea = gameObject.GetComponentInChildren<TMP_Text>();
      
        ClearConsole();
        textArea.text = ">Beginning of session: " + DateTime.Now;
    }

    public static void ClearConsole()
    {
        textArea.text = "";
    }

    public static void Log(string s)
    {
        textArea.text = textArea.text + "\n >" + s;
    }
}
