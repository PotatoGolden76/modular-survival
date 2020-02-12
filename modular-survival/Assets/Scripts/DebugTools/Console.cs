using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Console : MonoBehaviour
{
    private static TMP_Text tmp;

    void Start()
    {
        tmp = gameObject.GetComponentInChildren<TMP_Text>();
      
        ClearConsole();
        tmp.text = ">Beginning of session: " + System.DateTime.Now;
    }

    public static void ClearConsole()
    {
        tmp.text = "";
    }

    public static void Log(string s)
    {
        tmp.text = tmp.text + "\n >" + s;
    }
}
