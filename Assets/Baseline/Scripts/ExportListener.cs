using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class ExportListener : MonoBehaviour
{

    private Button button;

    private Mandelbrot fractalMandelbrot;

    private LogsController logsController;

    void Start()
    {
        button = GetComponent<Button>();
        fractalMandelbrot = GameObject.FindGameObjectWithTag("Mandelbrot").GetComponent<Mandelbrot>();
        logsController = GameObject.FindGameObjectWithTag("LogsController").GetComponent<LogsController>();
        button.onClick.AddListener(() => ExportImage());
    }


    private void ExportImage(){
        Sprite image = GameObject.FindGameObjectWithTag("Mandelbrot").GetComponent<Image>().sprite;
        Texture2D texture = image.texture;
        byte[] bytesToSave = texture.EncodeToPNG();
        File.WriteAllBytes( "/Users/adry/Desktop/test.png", bytesToSave);
        LogsController.UpdateLogs(new string[] {"Image exported successfully!"}, "#75FF00");
    }

    void Update()
    {
        if (fractalMandelbrot.GetFinished()){
            button.interactable = true;
        }else{
            button.interactable = false;
        }
    }
}
