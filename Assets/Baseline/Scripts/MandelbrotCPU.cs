using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;
using TMPro;

using System.Windows;




public class MandelbrotCPU : FractalCPU
{
    private static object lockObject = new object();
 
    private Color[] colors;

    public GameObject GOprogressBar;
    public GameObject GOpercentage;

    private Image progressBar;
    private TextMeshProUGUI percentage;

    CancellationTokenSource cancellationTokenSource;
    CancellationToken cancellationToken;

    void Awake(){
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;
        rp.fractalImage = GetComponent<Image>();
        rp.tex2D = new Texture2D((int) rp.pwidth, (int) rp.pheight);
        progressBar = GOprogressBar.transform.GetComponent<Image>();
        percentage = GOpercentage.transform.GetComponent<TextMeshProUGUI>();

    }
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
                cancellationTokenSource.Cancel();
            }
        }catch {
            Debug.Log("There is no Mandelbrot drawing coroutine running.");
        }finally {
            rp.finished = false;
            if (!rp.parallel){
                switch(fp.algorithm){
                    case "Escape Algorithm":
                        rp.drawingThread = StartCoroutine(Draw());
                        break;
                    case "Henriksen Algorithm":
                        rp.drawingThread = StartCoroutine(Draw());
                        break;
                    default:
                        rp.drawingThread = StartCoroutine(Draw());
                        break;

                }
            }else{
                switch(fp.algorithm){   
                    case "Escape Algorithm":
                        rp.drawingThread = StartCoroutine(DrawParallelized());
                        break;
                    case "Henriksen Algorithm":
                        rp.drawingThread = StartCoroutine(DrawParallelized());
                        break;
                    default:
                        rp.drawingThread = StartCoroutine(Draw());
                        break;

                }
            }
        }
    }

    public override void OnEnable(){
        //Draw();
    }

    private void UpdateProgress(){
        progressBar.fillAmount = GetProgress();
        percentage.text = (int)(GetProgress()*100) + "";
    }

    protected IEnumerator DrawParallelized(){
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;
        LogsController.UpdateLogs(new string[] {"Mandelbrot drawing corroutine started."}, "#ffffffff");
        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
        watch.Start();

        //BlockingCollection<ColorData> results = new BlockingCollection<ColorData>();
        CorrectAspectRatio();
        rp.xmin = - rp.xmax;
        rp.ymin = - rp.ymax;
        rp.tex2D = new Texture2D((int) rp.pwidth, (int) rp.pheight);
        rp.viewPortWidth = rp.xmax - rp.xmin;
        rp.viewPortHeight = rp.ymax - rp.ymin;
        ArrayList results = null;
        Task task = Task.Factory.StartNew(delegate {
            rp.count = 0;
            ArrayList res = new ArrayList();
            results = ArrayList.Synchronized( res );
            Parallel.For(0, rp.pwidth, (int x) => {
                cancellationToken.ThrowIfCancellationRequested();
                //for (int y = 0; y < rp.pheight; y++){
                Parallel.For(0, rp.pheight, (int y) => {
                    cancellationToken.ThrowIfCancellationRequested();
                    int i = 0;
                    Color value;
                        i = ComputeConvergence(x, y);  
                    value = PickColor(i);
                    results.Add(new ColorData(value, x, y));
                    lock(lockObject){
                        rp.count++;
                    }
                
                });
                //}
                
            });
        }, cancellationToken);
        while (!task.IsCompleted){
            if (!cancellationToken.IsCancellationRequested){
                UpdateProgress();
            }else{
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
        UpdateProgress();
        foreach(ColorData c in results){
            rp.tex2D.SetPixel(c.x, c.y, c.color);
        }
        rp.tex2D.Apply();
        rp.fractalImage.sprite = Sprite.Create(rp.tex2D, new Rect(0, 0, rp.tex2D.width, rp.tex2D.height), new UnityEngine.Vector2(0.5f, 0.5f)); 
        rp.finished = true;
        
        watch.Stop();
        LogsController.UpdateLogs(new string[] {"Mandelbrot drawing corroutine finished successfully in " + watch.ElapsedMilliseconds/1000.0+  "s!"}, "#75FF00");
        yield return new WaitForSeconds(0.5f);

    }



    protected new IEnumerator Draw(){
        LogsController.UpdateLogs(new string[] {"Mandelbrot drawing corroutine started."}, "#ffffffff");
        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
        watch.Start();
        int x, y, i = 0;
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
            if (x % 20 == 0){
                UpdateProgress();
                yield return new WaitForSeconds(0.001f);


            }
        }
        UpdateProgress();
        rp.tex2D.Apply();
        rp.fractalImage.sprite = Sprite.Create(rp.tex2D, new Rect(0, 0, rp.tex2D.width, rp.tex2D.height), new UnityEngine.Vector2(0.5f, 0.5f)); 
        yield return new WaitForSeconds(0.5f);
        rp.finished = true;
        watch.Stop();
        LogsController.UpdateLogs(new string[] {"Mandelbrot drawing corroutine finished successfully in " + watch.ElapsedMilliseconds/1000.0+  "s!"}, "#75FF00");
    }

    private int ComputeConvergence(int x, int y){
        Complex z, c;
        int i;
        lock(this){
            rp.viewPortX = rp.xmin + ((double) x / rp.pwidth) * rp.viewPortWidth + rp.panX;
            rp.viewPortY = rp.ymin + ((double) y / rp.pheight) * rp.viewPortHeight + rp.panY;
            z = new Complex(0.0f, 0.0f);
            c = new Complex(rp.viewPortX, rp.viewPortY);
            i = 0;
        }
        while (Complex.Abs(z) < fp.threshold && i < fp.maxIters){
            //z = Complex.Pow(z, fp.degree) + c;
            z = new Complex(z.Real * z.Real - z.Imaginary * z.Imaginary, 2.0 * z.Real * z.Imaginary) + c;
            //lock(this){i++;}
            i++;
        }
        return i;
    }

    private int ComputeConvergenceNewton(int x, int y){
        rp.viewPortX = rp.xmin + ((double) x / rp.pwidth) * rp.viewPortWidth + rp.panX;
        rp.viewPortY = rp.ymin + ((double) y / rp.pheight) * rp.viewPortHeight + rp.panY;
        Complex z = new Complex(0.0f, 0.0f);
        Complex c = new Complex(rp.viewPortX, rp.viewPortY);
        int i = 0;
        double tol = 1e-6;
        Complex r1, r2, r3;
        r1 = new Complex(1.0, 0.0);
        r2 = new Complex(-0.5, 0.86603);
        r3 = new Complex(-0.5, -0.86603);
        while (Complex.Abs(z - r1) >= tol && Complex.Abs(z - r2) >= tol && Complex.Abs(z - r3) >= tol && i < fp.maxIters){
            //z = Complex.Pow(z, fp.degree) + c;
            if (Complex.Abs(z) > 0){
                z = z - ((Complex.Pow(z, 3) - 1) / (Complex.Pow(z, 2) * 3.0));
            }
            i++;
        }
        if (Complex.Abs(z - r1) < tol){
            return 20;
        }
        if (Complex.Abs(z - r2) < tol){
            return 40;
        }
        if (Complex.Abs(z - r3) < tol){
            return 60;
        }
        return 60;
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
