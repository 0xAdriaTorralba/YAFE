using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LogsController : MonoBehaviour
{
    private GameObject logsBar;
    private Text textLogs;

    private Queue<string> logs;

    void Awake(){
        // Gather all info about logs before deactivate it.
        logsBar = GameObject.FindGameObjectWithTag("Logs");
        textLogs = GameObject.FindGameObjectWithTag("TextLogs").GetComponent<Text>();
        DeactivateLogs();
        logs = new Queue<string>();
    }

    public void ActivateLogs(){
        logsBar.SetActive(true);
    }

    public void DeactivateLogs(){
        logsBar.SetActive(false);
    }

    public void UpdateLogs(string[] newLogs, String color){
        StartCoroutine(DisplayNextLog(newLogs, color));
    }

    private IEnumerator DisplayNextLog(string[] newLogs, String color){
        yield return new WaitForEndOfFrame();

        logs.Clear();
        foreach(string log in newLogs){
            logs.Enqueue(log);
        }

        if (logs.Count == 0){
            yield break;
        }
        ActivateLogs();
        DateTime date = DateTime.Now;
        string newLog = logs.Dequeue();
        newLog = "<color=" + color + ">" + '<' + DateTime.Now.ToString("HH:mm:ss") + "> " + newLog + "</color>";
        string[] currentLogs = textLogs.text.Split('\n', (char) StringSplitOptions.RemoveEmptyEntries);
        textLogs.text = currentLogs[1] + '\n' + currentLogs[2] + '\n' + currentLogs[3] + '\n' + currentLogs[4] + '\n' + newLog;
    }
}
