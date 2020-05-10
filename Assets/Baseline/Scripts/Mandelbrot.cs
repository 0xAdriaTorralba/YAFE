using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;


public class Mandelbrot : Fractal
{
    private Color[] colors;
    void Start(){
        colors = new Color[256];
        for(int i = 0; i < 255; i++){
            colors[i] = Color.HSVToRGB((float)i/256.0f, 1.0f, 1.0f);
        }
        colors[255] = Color.black;
    }

    public override void StartDraw(){
        try{
            StopCoroutine(rp.drawingThread);
            if (!rp.finished){
                LogsController.UpdateLogs(new string[] {"Mandelbrot drawing corroutine stopped."}, "#FFA600");
            }
        }catch {
            Debug.Log("There is no Mandelbrot drawing coroutine running.");
        }finally {
            rp.finished = false;
            switch(fp.algorithm){
                case "Escape Algorithm":
                    rp.drawingThread = StartCoroutine(Draw());
                    break;
                case "Another Algorithm":
                    break;
                default:
                    rp.drawingThread = StartCoroutine(Draw());
                    break;

            }
        }
    }

    public override void OnEnable(){
        //Draw();
    }

    protected new IEnumerator Draw(){
        LogsController.UpdateLogs(new string[] {"Mandelbrot drawing corroutine started."}, "#ffffffff");
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
                i = ComputeConvergence(x, y);
                value = PickColor(i);
                rp.count++;
                rp.tex2D.SetPixel((int) x, (int) y, value);
                // For display purposes
                
            }
             if (x % 10 == 0){
            //     rp.tex2D.Apply();
            //     rp.fractalImage.sprite = Sprite.Create(rp.tex2D, new Rect(0, 0, rp.tex2D.width, rp.tex2D.height), new UnityEngine.Vector2(0.5f, 0.5f)); 
            //     yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(0.0001f);
            }
        }
        rp.tex2D.Apply();
        rp.fractalImage.sprite = Sprite.Create(rp.tex2D, new Rect(0, 0, rp.tex2D.width, rp.tex2D.height), new UnityEngine.Vector2(0.5f, 0.5f)); 
        yield return new WaitForSeconds(0.5f);
        rp.finished = true;
        watch.Stop();
        LogsController.UpdateLogs(new string[] {"Mandelbrot drawing corroutine finished successfully in " + watch.ElapsedMilliseconds/1000.0+  "s!"}, "#75FF00");
    }

    private int ComputeConvergence(int x, int y){
        rp.viewPortX = rp.xmin + ((double) x / rp.pwidth) * rp.viewPortWidth + rp.panX;
        rp.viewPortY = rp.ymin + ((double) y / rp.pheight) * rp.viewPortHeight + rp.panY;
        Complex z = new Complex(0.0f, 0.0f);
        Complex c = new Complex(rp.viewPortX, rp.viewPortY);
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
