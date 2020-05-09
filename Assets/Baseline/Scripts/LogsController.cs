using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LogsController : MonoBehaviour
{
    private static Text textLogs;

    private static Queue<string> logs;
    private static LogsController instance;

    void Awake(){
        instance = this;
    }

    public static void UpdateLogs(string[] newLogs, String color){
        if (logs == null){
            logs = new Queue<string>();
        }
        textLogs = GameObject.FindGameObjectWithTag("TextLogs").GetComponent<Text>();
        instance.StartCoroutine(DisplayNextLog(newLogs, color));
    }

    private static IEnumerator DisplayNextLog(string[] newLogs, String color){
        yield return new WaitForEndOfFrame();

        logs.Clear();
        foreach(string log in newLogs){
            logs.Enqueue(log);
        }

        if (logs.Count == 0){
            yield break;
        }
        DateTime date = DateTime.Now;
        string newLog = logs.Dequeue();
        newLog = "<color=" + color + ">" + '<' + DateTime.Now.ToString("HH:mm:ss") + "> " + newLog + "</color>";
        string[] currentLogs = textLogs.text.Split('\n', (char) StringSplitOptions.RemoveEmptyEntries);
        textLogs.text = currentLogs[1] + '\n' + currentLogs[2] + '\n' + currentLogs[3] + '\n' + currentLogs[4] + '\n' + newLog;
    }
}
