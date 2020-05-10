using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using System;


public class Julia : Fractal
{

    private Color[] colors;
    void Start(){
        colors = new Color[256];
        for(int i = 0; i < 255; i++){
            colors[i] = Color.HSVToRGB((float)i/256.0f, 1.0f, 1.0f);
        }
        colors[255] = Color.black;
    }
    public double reZ;
    public double imZ;
    public override void StartDraw(){
        throw new Exception("(!) You should call StartDraw(double reZ, double imZ) instead.");
    }

    public void StartDraw(double rez, double imz){
        try{
            StopCoroutine(rp.drawingThread);
            if (!rp.finished){
                LogsController.UpdateLogs(new string[] {"Julia drawing corroutine stopped."}, "#FFA600");
            }
        }catch {
            Debug.Log("There is no Julia drawing coroutine running.");
        }finally {
            this.reZ = rez;
            this.imZ = imz;
            rp.finished = false;
            switch(fp.algorithm){
                case "Escape Algorithm":
                    rp.drawingThread = StartCoroutine(Draw(rez, imz));
                    break;
                case "Another Algorithm":
                    break;
                default:
                    rp.drawingThread = StartCoroutine(Draw(rez, imz));
                    break;

            }
        }
    }

    public void RedrawCurrent(){
        try{
            StopCoroutine(rp.drawingThread);
            if (!rp.finished){
                LogsController.UpdateLogs(new string[] {"Julia drawing corroutine stopped."}, "#FFA600");
            }
        }catch {
            Debug.Log("There is no Julia drawing coroutine running.");
        }finally {
            rp.finished = false;
            switch(fp.algorithm){
                case "Escape Algorithm":
                    rp.drawingThread = StartCoroutine(Draw(reZ, imZ));
                    break;
                case "Another Algorithm":
                    break;
                default:
                    rp.drawingThread = StartCoroutine(Draw(reZ, imZ));
                    break;

            }
        }
    }

    public override void OnEnable(){
        //Draw(reZ, imZ);
    }

    protected IEnumerator Draw(double reZ, double imZ){
        LogsController.UpdateLogs(new string[] {"Julia drawing corroutine started."}, "#ffffffff");
        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
        watch.Start();
        int x, y, i;
        rp.count = 0;
        CorrectAspectRatio();
        rp.xmin = - rp.xmax;
        rp.ymin = - rp.ymax;
        rp.tex2D = new Texture2D((int) rp.pwidth, (int) rp.pheight);
        rp.viewPortWidth = rp.xmax - rp.xmin;
        rp.viewPortHeight = rp.ymax - rp.ymin;
        for (x = 0; x < rp.pwidth; x++){
            for (y = 0; y < rp.pheight; y++){
                Color value;
                i = ComputeConvergence(x, y, reZ, imZ);
                value = PickColor(i);
                rp.count++;
                rp.tex2D.SetPixel((int) x, (int) y, value);
                // For display purposes
                
                }
                if (x % 10 == 0){
                    // rp.tex2D.Apply();
                    // rp.fractalImage.sprite = Sprite.Create(rp.tex2D, new Rect(0, 0, rp.tex2D.width, rp.tex2D.height), new UnityEngine.Vector2(0.5f, 0.5f)); 
                    // yield return new WaitForEndOfFrame();
                    yield return new WaitForSeconds(0.0001f);
            }
        }
        rp.tex2D.Apply();
        rp.fractalImage.sprite = Sprite.Create(rp.tex2D, new Rect(0, 0, rp.tex2D.width, rp.tex2D.height), new UnityEngine.Vector2(0.5f, 0.5f)); 
        yield return new WaitForSeconds(0.5f);
        rp.finished = true;
        watch.Stop();
        LogsController.UpdateLogs(new string[] {"Julia drawing corroutine finished successfully in " + watch.ElapsedMilliseconds/1000.0+  "s!"}, "#75FF00");
    }

    private int ComputeConvergence(int x, int y, double reZ, double imZ){
        rp.viewPortX = rp.xmin + ((double) x / rp.pwidth) * rp.viewPortWidth + rp.panX;
        rp.viewPortY = rp.ymin + ((double) y / rp.pheight) * rp.viewPortHeight + rp.panY;
        Complex z = new Complex(rp.viewPortX, rp.viewPortY);
        Complex c = new Complex(reZ, imZ);
        int i = 0;
        while (Complex.Abs(z) < 2 && i < fp.maxIters){
            z = Complex.Pow(z, 2) + c;
            i++;
        }
        return i;
    }

    private Color PickColor(int i){
        switch (fp.colorMap){
            case "Colormap 1 (Sin)":
                if (i == fp.maxIters){
                    return new Color(0.0f, 0.0f, 0.0f, 1.0f);
                }else{
                    return new Color(Mathf.Sin((float)i/4) / 4.0f + 0.75f, Mathf.Sin((float)i/5) / 4.0f + 0.75f, Mathf.Sin((float)i/7) / 4.0f + 0.75f, 1.0f);
                }
            case "Colormap 2 (Cos)":
                if (i == fp.maxIters){
                    return new Color(0.0f, 0.0f, 0.0f, 1.0f);
                }else{
                    return new Color(Mathf.Cos((float)i/4) / 4.0f + 0.75f, Mathf.Cos((float)i/5) / 4.0f + 0.75f, Mathf.Cos((float)i/7) / 4.0f + 0.75f, 1.0f);
                }
            case "Colormap 3 (Tan)":
                if (i == fp.maxIters){
                    return new Color(0.0f, 0.0f, 0.0f, 1.0f);
                }else{
                    return new Color(Mathf.Tan((float)i/4) / 4.0f + 0.75f, Mathf.Tan((float)i/5) / 4.0f + 0.75f, Mathf.Tan((float)i/7) / 4.0f + 0.75f, 1.0f);
                }
            case "Gradient":
                return colors[(int)(255 * (double) i / fp.maxIters)];
            case "Black and White":
                if (i == fp.maxIters){
                    return new Color(0.0f, 0.0f, 0.0f, 1.0f); 
                }else{
                    return new Color(1.0f, 1.0f, 1.0f, 1.0f); 
                }
            default:
                if (i == fp.maxIters){
                    return new Color(0.0f, 0.0f, 0.0f, 1.0f);
                }else{
                    return new Color(Mathf.Sin((float)i/4) / 4.0f + 0.75f, Mathf.Sin((float)i/5) / 4.0f + 0.75f, Mathf.Sin((float)i/7) / 4.0f + 0.75f, 1.0f);
                }
        }
    }
}
