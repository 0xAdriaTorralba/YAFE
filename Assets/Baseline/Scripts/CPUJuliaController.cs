﻿using System.Collections;
using UnityEngine;
using System.Numerics;
using UnityEngine.UI;

public class CPUJuliaController : MonoBehaviour
{
    private Texture2D brush;
    public int maxIters = 10;  
    public int pwidth = 2000;
    public int pheight = 2000;
    private double xmin;
    public double xmax = 2f;
    private double ymin;
    public double ymax = 2f;
    public double panX = 0.5f;
    public double panY = 0.5f;
    double viewPortWidth;
    double viewPortHeight;

    public int positionX, positionY;
    
    private int count;
    private bool finished;

    double viewPortX, viewPortY;

    private Coroutine drawingThread;

    private double rez, imz;
    private LogsController logsController;
    private Image juliaImage;
    void Awake()
    {   
        finished = true;
        logsController = GameObject.FindGameObjectWithTag("LogsController").GetComponent<LogsController>();
        juliaImage = GameObject.FindGameObjectWithTag("Julia").GetComponent<Image>();
        //results = new Color[pwidth, pheight];
        //resultArray = new NativeArray<Color>(pwidth * pheight, Allocator.Persistent);
        double ratio = pwidth / pheight;
        if (ratio > 1f){
            xmax *= ratio;
        }else{
            ymax /= ratio;
        }
        brush = new Texture2D((int)pwidth, (int)pheight);
        
        
        //StartCoroutine(ChangeTheColor());
        //ChangeTheColor();
        //ChangeTheColorParallel();
    }

    public void Start(){
        drawingThread = null;
        finished = true;
    }

    public void StartDraw(double rez, double imz){
        try{
            StopCoroutine(drawingThread);
        }catch {} 
        finished = false;
        this.rez = rez;
        this.imz = imz;
        drawingThread =  StartCoroutine(ChangeTheColor(rez, imz));
    }

    public void RedrawCurrent(){
        if (!finished){
            StopCoroutine(drawingThread);
        }
        finished = false;
        drawingThread =  StartCoroutine(ChangeTheColor(rez, imz));
    }

    public void StopDrawingCorroutine(){
        try{
            StopCoroutine(drawingThread);
            logsController.UpdateLogs(new string[] {"Julia drawing corroutine stopped."}, "#ff0000ff");
            count = 0;
            brush = new Texture2D((int)pwidth, (int)pheight);
        }catch {}
    }


    public float GetProgress(){
        return (float)count/(pwidth * pheight);
    }

    public bool GetFinished(){
        return finished;
    }

    void FixedUpdate(){
        //ChangeTheColor();
    }
    // TODO buscar OpenMP per C#.
   /* void ChangeTheColorParallel()
    {
        
        double i = 0;
        count = 0;

        int y;
        
        Parallel.For(0, pwidth, x =>
            {
                for (y = 0; y < pheight; y++){
                    Color value;
                    
                    viewPortX = xmin + (x / pwidth) * viewPortWidth + panX;
                    viewPortY = ymin + (y / pheight) * viewPortHeight + panY;
                    Complex z = new Complex(0.0f, 0.0f);
                    Complex c = new Complex(viewPortX, viewPortY);
                    i = 0;
                    while (Complex.Abs(z) < 25 && i < maxIters){
                        //z = -4.0f*Complex.Pow(z, 2)*(z-1);
                        z = Complex.Pow(z, 2) + c;
                        i++;
                    }
                    
                    if (i == maxIters){
                        value = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                    }else{
                        value = new Color(Mathf.Sin((float)i/4) / 4.0f + 0.75f, Mathf.Sin((float)i/5) / 4.0f + 0.75f, Mathf.Sin((float)i/7) / 4.0f + 0.75f, 1.0f);
                    }
                    Interlocked.Increment(ref count);
                    //brush.SetPixel((int)x, (int)y, value);
                    //results[(int)x, (int)y] = value;
                    lock(mutex){
                        resultArray[x * pheight + y] = value;
                    }
                    
                    // if (count % 100 == 0){ 
                    //     yield return new WaitForEndOfFrame();
                    // }
                }
            });
        
        for (int x = 0; x < pwidth; x++){
            for (y = 0; y < pheight; y++){
                brush.SetPixel((int) x, (int) y, resultArray[y *pwidth + x]);
            }
        }

        brush.Apply();

        // for (double x = 0; x < pwidth; x++){
        //     for (double y = 0; y < pheight; y++){
        //         Color value;
                
        //         viewPortX = xmin + (x / pwidth) * viewPortWidth + panX;
        //         viewPortY = ymin + (y / pheight) * viewPortHeight + panY;
        //         Complex z = new Complex(0.0f, 0.0f);
        //         Complex c = new Complex(viewPortX, viewPortY);
        //         i = 0;
        //         while (Complex.Abs(z) < 25 && i < maxIters){
        //             //z = -4.0f*Complex.Pow(z, 2)*(z-1);
        //             z = Complex.Pow(z, 2) + c;
        //             i++;
        //         }
                
        //         if (i == maxIters){
        //             value = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        //         }else{
        //             value = new Color(Mathf.Sin((float)i/4) / 4.0f + 0.75f, Mathf.Sin((float)i/5) / 4.0f + 0.75f, Mathf.Sin((float)i/7) / 4.0f + 0.75f, 1.0f);
        //         }
        //         count++;
        //         brush.SetPixel((int)x, (int)y, value);
        //         if (count % 100 == 0){ 
        //             yield return new WaitForEndOfFrame();
        //         }
        //     }
        // }

    }*/

    IEnumerator ChangeTheColor(double rez, double imz){
        logsController.UpdateLogs(new string[] {"Julia drawing corroutine started."}, "	#ffffffff");

        yield return null;
        int i;
        int x, y;
        xmin = -xmax;
        ymin = -ymax;
        brush = new Texture2D((int)pwidth, (int)pheight);
        viewPortWidth = xmax - xmin;
        viewPortHeight = ymax - ymin;
        finished = false;
        count = 0;
        for (x = 0; x < pwidth; x++){
            for (y = 0; y < pheight; y++){
                Color value;
                
                viewPortX = xmin + ((double)x / pwidth) * viewPortWidth + panX;
                viewPortY = ymin + ((double)y / pheight) * viewPortHeight + panY;
                Complex z = new Complex(viewPortX, viewPortY);
                Complex c = new Complex(rez, imz);
                i = 0;
                while (Complex.Abs(z) < 25 && i < maxIters){
                    //z = -4.0f*Complex.Pow(z, 2)*(z-1);
                    z = Complex.Pow(z, 2) + c;
                    i++;
                    //Debug.Log(i);
                }
                
                if (i == maxIters){
                    value = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                }else{
                    value = new Color(Mathf.Sin((float)i/4) / 4.0f + 0.75f, Mathf.Sin((float)i/5) / 4.0f + 0.75f, Mathf.Sin((float)i/7) / 4.0f + 0.75f, 1.0f);
                    //value = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }
                count++;
                //Debug.Log(value);
                brush.SetPixel((int)x, (int)y, value);
                // if (count % 100 == 0){ 
                //     yield return new WaitForEndOfFrame();
                // }
            }
            if (x % 10 == 0){ 
                brush.Apply();
                juliaImage.sprite = Sprite.Create(brush, new Rect(0, 0, brush.width, brush.height), new UnityEngine.Vector2(0.5f, 0.5f), 100f);
                yield return new WaitForEndOfFrame();
            }
        }
        brush.Apply();
        juliaImage.sprite = Sprite.Create(brush, new Rect(0, 0, brush.width, brush.height), new UnityEngine.Vector2(0.5f, 0.5f), 100f);
        finished = true;

        logsController.UpdateLogs(new string[] {"Julia drawing corroutine finished successfully."}, "#00ff00ff");

        
    }
        
    void OnGUI()
    {
        //GUILayout.Label(brush);
        //GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, pwidth, pheight), brush);
        //GUI.DrawTexture(new Rect(Screen.width - positionX, Screen.height-positionY, pwidth, pheight), brush);
    }
}
