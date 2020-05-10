using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;


public class Mandelbrot : Fractal
{
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
                rp.viewPortX = rp.xmin + ((double) x / rp.pwidth) * rp.viewPortWidth + rp.panX;
                rp.viewPortY = rp.ymin + ((double) y / rp.pheight) * rp.viewPortHeight + rp.panY;
                Complex z = new Complex(0.0f, 0.0f);
                Complex c = new Complex(rp.viewPortX, rp.viewPortY);
                i = 0;
                while (Complex.Abs(z) < 2 && i < fp.maxIters){
                    z = Complex.Pow(z, 2) + c;
                    i++;
                }

                if (i == fp.maxIters){
                    value = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                }else{
                    switch (fp.colorMap){
                        case "Colormap 1":
                            value = new Color(Mathf.Sin((float)i/4) / 4.0f + 0.75f, Mathf.Sin((float)i/5) / 4.0f + 0.75f, Mathf.Sin((float)i/7) / 4.0f + 0.75f, 1.0f);
                            break;
                        case "Black and White":
                            value = new Color(1.0f, 1.0f, 1.0f, 1.0f); 
                            break;
                        default:
                            value = new Color(Mathf.Sin((float)i/4) / 4.0f + 0.75f, Mathf.Sin((float)i/5) / 4.0f + 0.75f, Mathf.Sin((float)i/7) / 4.0f + 0.75f, 1.0f);
                            break;
                    }
                }
                rp.count++;
                rp.tex2D.SetPixel((int) x, (int) y, value);
            }
            
            if (x % 188 == 0){
                rp.tex2D.Apply();
                rp.fractalImage.sprite = Sprite.Create(rp.tex2D, new Rect(0, 0, rp.tex2D.width, rp.tex2D.height), new UnityEngine.Vector2(0.5f, 0.5f)); 
                yield return new WaitForEndOfFrame();
            }
        }
        rp.tex2D.Apply();
        rp.fractalImage.sprite = Sprite.Create(rp.tex2D, new Rect(0, 0, rp.tex2D.width, rp.tex2D.height), new UnityEngine.Vector2(0.5f, 0.5f)); 
        yield return new WaitForSeconds(0.5f);
        rp.finished = true;
        watch.Stop();
        LogsController.UpdateLogs(new string[] {"Mandelbrot drawing corroutine finished successfully in " + watch.ElapsedMilliseconds/1000.0+  "s!"}, "#75FF00");

    }
}
