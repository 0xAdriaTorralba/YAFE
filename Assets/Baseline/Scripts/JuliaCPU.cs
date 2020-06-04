using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using System;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;


public class JuliaCPU : FractalCPU
{
    private static object lockObject = new object();

    private Texture2D originalImage;

    private Color[] colors;

    public GameObject GOprogressBar;
    public GameObject GOpercentage;

    private Image progressBar;
    private TextMeshProUGUI percentage;

    private string format = "F8";

    private int numImages = 1;


    void Awake(){
        rp.fractalImage = GetComponent<Image>();
        rp.tex2D = new Texture2D((int) rp.pwidth, (int) rp.pheight);
        originalImage = new Texture2D((int) rp.pwidth, (int) rp.pheight);
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
            rp.finished = false;
            if (!rp.parallel){
                switch(fp.algorithm){
                    case "Escape Algorithm":
                        rp.drawingThread = StartCoroutine(Draw(reZ, imZ));
                        break;
                    case "Henriksen Algorithm":
                        rp.drawingThread = StartCoroutine(DrawHenriksen(reZ, imZ));
                        break;
                    default:
                        rp.drawingThread = StartCoroutine(Draw(reZ, imZ));
                        break;

                }
            }else{
                switch(fp.algorithm){   
                    case "Escape Algorithm":
                        rp.drawingThread = StartCoroutine(DrawParallelized(reZ, imZ));
                        break;
                    case "Henriksen Algorithm":
                        rp.drawingThread = StartCoroutine(DrawHenriksenParallelized(reZ, imZ));
                        break;
                    default:
                        rp.drawingThread = StartCoroutine(DrawParallelized(reZ, imZ));
                        break;

                }
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
            if (!rp.parallel){
                switch(fp.algorithm){
                    case "Escape Algorithm":
                        rp.drawingThread = StartCoroutine(Draw(reZ, imZ));
                        break;
                    case "Henriksen Algorithm":
                        rp.drawingThread = StartCoroutine(DrawHenriksen(reZ, imZ));
                        break;
                    default:
                        rp.drawingThread = StartCoroutine(Draw(reZ, imZ));
                        break;

                }
            }else{
                switch(fp.algorithm){   
                    case "Escape Algorithm":
                        rp.drawingThread = StartCoroutine(DrawParallelized(reZ, imZ));
                        break;
                    case "Henriksen Algorithm":
                        rp.drawingThread = StartCoroutine(DrawHenriksenParallelized(reZ, imZ));
                        break;
                    default:
                        rp.drawingThread = StartCoroutine(DrawParallelized(reZ, imZ));
                        break;

                }
            }
        }
    }

    public void ResetRenderParameters(){
        rp.xmax = 2.0;
        rp.ymax = 2.0;
        rp.panX = 0.0;
        rp.panY = 0.0;

    }

    public override void OnEnable(){
        //Draw(reZ, imZ);
    }

    private void UpdateProgress(){
        progressBar.fillAmount = GetProgress();
        percentage.text = (int)(GetProgress()*100) + "";
    }

    protected IEnumerator DrawHenriksenParallelized(double reZ, double imZ){
        LogsController.UpdateLogs(new string[] {"Julia drawing corroutine started."}, "#ffffffff");
        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
        watch.Start();
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
            results = ArrayList.Synchronized(res);
        Parallel.For(0, rp.pwidth, (int x) => {

            Parallel.For(0, rp.pheight, (int y) => {
                Color value;
                int i;
                value = ComputeConvergenceHenriksenColor(x, y, reZ, imZ);
                //value = PickColor(i);
                results.Add(new ColorData(value, x, y));
                lock(lockObject){
                    rp.count++;
                }
                
                
            });
            
        } );} );
        while (!task.IsCompleted){
            UpdateProgress();
            yield return new WaitForSeconds(0.1f);
        }
        UpdateProgress();
        foreach(ColorData c in results){
            rp.tex2D.SetPixel(c.x, c.y, c.color);
        }
        rp.tex2D.Apply();
        rp.fractalImage.sprite = Sprite.Create(rp.tex2D, new Rect(0, 0, rp.tex2D.width, rp.tex2D.height), new UnityEngine.Vector2(0.5f, 0.5f)); 
        yield return new WaitForSeconds(0.5f);
        rp.finished = true;
        watch.Stop();
        Color[] aux = rp.tex2D.GetPixels();
        originalImage = new Texture2D(rp.pwidth, rp.pheight);
        originalImage.SetPixels(aux);
        LogsController.UpdateLogs(new string[] {"Julia drawing corroutine finished successfully in " + watch.ElapsedMilliseconds/1000.0+  "s!"}, "#75FF00");
        
    }

    protected IEnumerator DrawHenriksen(double reZ, double imZ){
        LogsController.UpdateLogs(new string[] {"Julia drawing corroutine started."}, "#ffffffff");
        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
        watch.Start();
        int i;
        rp.count = 0;
        CorrectAspectRatio();
        rp.xmin = - rp.xmax;
        rp.ymin = - rp.ymax;
        rp.tex2D = new Texture2D((int) rp.pwidth, (int) rp.pheight);
        rp.viewPortWidth = rp.xmax - rp.xmin;
        rp.viewPortHeight = rp.ymax - rp.ymin;
        ArrayList res = new ArrayList();
        for(int x = 0; x < rp.pwidth; x++){
            for (int y = 0; y < rp.pheight; y++){
                Color value;
                value = ComputeConvergenceHenriksenColor(x, y, reZ, imZ);
                //value = PickColor(i);
                rp.tex2D.SetPixel(x, y, value);
                rp.count++;
                
            }
            // For display purposes
            if (x % 10 == 0){
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
        Color[] aux = rp.tex2D.GetPixels();
        originalImage = new Texture2D(rp.pwidth, rp.pheight);
        originalImage.SetPixels(aux);
        LogsController.UpdateLogs(new string[] {"Julia drawing corroutine finished successfully in " + watch.ElapsedMilliseconds/1000.0+  "s!"}, "#75FF00");
        
    }

    
    private void DrawLine(Texture2D tex, UnityEngine.Vector2 p1, UnityEngine.Vector2 p2, Color col)
    {
        UnityEngine.Vector2 t = p1;
        float frac = 1/Mathf.Sqrt (Mathf.Pow (p2.x - p1.x, 2) + Mathf.Pow (p2.y - p1.y, 2));
        float ctr = 0;
     
        while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y) {
            t = UnityEngine.Vector2.Lerp(p1, p2, ctr);
            ctr += frac;
            if ((int)t.x > tex.width || (int) t.x < 0 || (int) t.y > tex.height || (int) t.y < 0){
                continue;
            }
            tex.SetPixel((int)t.x, (int)t.y, col);
        } 
    }

    protected IEnumerator DrawParallelized(double reZ, double imZ){
        LogsController.UpdateLogs(new string[] {"Julia drawing corroutine started."}, "#ffffffff");
        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
        watch.Start();
        CorrectAspectRatio();
        rp.xmin = - rp.xmax;
        rp.ymin = - rp.ymax;
        rp.tex2D = new Texture2D((int) rp.pwidth, (int) rp.pheight);
        rp.viewPortWidth = rp.xmax - rp.xmin;
        rp.viewPortHeight = rp.ymax - rp.ymin;

        ArrayList results = null;
        Task task = Task.Factory.StartNew(delegate {
            ArrayList res = new ArrayList();
            results = ArrayList.Synchronized( res );
            rp.count = 0;

            Parallel.For(0, rp.pwidth, (int x) => {
                
                Parallel.For(0, rp.pheight, (int y) => {
                    Color value;
                    int i = 0;
                    i = ComputeConvergence(x, y, reZ, imZ);
                    value = PickColor(i);
                    
                    results.Add(new ColorData(value, x, y));
                    lock (lockObject){
                        rp.count++;
                    } 
                    
                    
                });
                
            } );
        } );
        while (!task.IsCompleted){
            UpdateProgress();
            yield return new WaitForSeconds(0.1f);
        }
        UpdateProgress();
        foreach(ColorData c in results){
            rp.tex2D.SetPixel(c.x, c.y, c.color);
        }
        rp.tex2D.Apply();
        rp.fractalImage.sprite = Sprite.Create(rp.tex2D, new Rect(0, 0, rp.tex2D.width, rp.tex2D.height), new UnityEngine.Vector2(0.5f, 0.5f)); 
        yield return new WaitForSeconds(0.5f);
        rp.finished = true;
        watch.Stop();
        Color[] aux = rp.tex2D.GetPixels();
        originalImage = new Texture2D(rp.pwidth, rp.pheight);
        originalImage.SetPixels(aux);
        LogsController.UpdateLogs(new string[] {"Julia drawing corroutine finished successfully in " + watch.ElapsedMilliseconds/1000.0+  "s!"}, "#75FF00");
        
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
                

                }
                // For display purposes
                if (x % 10 == 0){
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
        Color[] aux = rp.tex2D.GetPixels();
        originalImage = new Texture2D(rp.pwidth, rp.pheight);
        originalImage.SetPixels(aux);
        LogsController.UpdateLogs(new string[] {"Julia drawing corroutine finished successfully in " + watch.ElapsedMilliseconds/1000.0+  "s!"}, "#75FF00");
        
    }

    public void CalculateImageAndDrawImage(double reZ, double imZ, int x, int y){
        int cImageReZ, cImageImZ;
        Complex originalPoint = new Complex(reZ, imZ);
        int oldX = x, oldY = y;
        Complex z = originalPoint;
        Color[] aux = rp.tex2D.GetPixels();
        originalImage = new Texture2D(rp.pwidth, rp.pheight);
        originalImage.SetPixels(aux);
        originalImage.Apply();
        for (int k = 0; k < numImages; k++){
            z = Complex.Pow(z, fp.degree) + new Complex(this.reZ, this.imZ);
            cImageReZ = (int)((z.Real - rp.panX - rp.xmin) * (rp.pwidth / rp.viewPortWidth));
            cImageImZ = (int)((z.Imaginary - rp.panY - rp.ymin) * (rp.pheight / rp.viewPortHeight));
            DrawLine(rp.tex2D, new UnityEngine.Vector2 (oldX, oldY), new UnityEngine.Vector2 (cImageReZ, cImageImZ), Color.white);
            oldX = cImageReZ;
            oldY = cImageImZ;
            if (Complex.Abs(z) > 2){
                break;
            }
        }
        rp.tex2D.Apply();
        rp.fractalImage.sprite = Sprite.Create(rp.tex2D, new Rect(0, 0, rp.tex2D.width, rp.tex2D.height), new UnityEngine.Vector2(0.5f, 0.5f)); 
    }

    public void RestoreImage(){
        rp.tex2D = originalImage;
        rp.fractalImage.sprite = Sprite.Create(rp.tex2D, new Rect(0, 0, rp.tex2D.width, rp.tex2D.height), new UnityEngine.Vector2(0.5f, 0.5f)); 
    }

    public int iters = 1000;
    public int threshold = 1000;
    public double tol = 1;

    private Color ComputeConvergenceHenriksenColor(int x, int y, double reZ, double imZ){
        Complex z, w, dz, epsilon, c;
        bool orbitFound;
        int i;
        lock(lockObject){
            rp.viewPortX = rp.xmin + ((double) x / rp.pwidth) * rp.viewPortWidth + rp.panX;
            rp.viewPortY = rp.ymin + ((double) y / rp.pheight) * rp.viewPortHeight + rp.panY;
            z = new Complex(rp.viewPortX, rp.viewPortY);
            w = new Complex(rp.viewPortX, rp.viewPortY);
            dz = new Complex(1.0, 0.0);
            epsilon = new Complex(50.0, 50.0);
            orbitFound = false;
            c = new Complex(reZ, imZ);
            i = 0;
            tol = rp.viewPortWidth / (double)1e4;
        }
        while (
                i < iters && 
                !orbitFound
            ){
            dz = fp.degree * Complex.Pow(z, fp.degree - 1) * dz;
            z = Complex.Pow(z, fp.degree) + c;

            if (Complex.Abs(z) > threshold){
                return PickColor(i);
            }

            if (Complex.Abs(dz - 1) > 1e-12){
                epsilon = (w - z) / (dz - 1.0);
            }else{
                continue;
                
            }
            if ((Math.Abs(epsilon.Real) < tol && Math.Abs(epsilon.Imaginary) < tol)){
                orbitFound = true;
                break;
            }
            i++;

        }

        // condition i > 5 (for instance) in order to avoid find fixed points inside the filled Julia set.
        if (orbitFound && i > 5){
            return Color.black;
        }else{
            return new Color(184/255.0f, 28/255.0f, 74/255.0f);
        }
    }

    private int ComputeConvergence(int x, int y, double reZ, double imZ){
        Complex z, c;
        int i;
        lock(lockObject){
            rp.viewPortX = rp.xmin + ((double) x / rp.pwidth) * rp.viewPortWidth + rp.panX;
            rp.viewPortY = rp.ymin + ((double) y / rp.pheight) * rp.viewPortHeight + rp.panY;
            z = new Complex(rp.viewPortX, rp.viewPortY);
            c = new Complex(reZ, imZ);
            i = 0;
        }
        while (Complex.Abs(z) < fp.threshold && i < fp.maxIters){
            z = Complex.Pow(z, fp.degree) + c;
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

    public void SetNumImages(int numImages){
        this.numImages = numImages;
    }

    public int GetNumImages(){
        return this.numImages;
    }
}
