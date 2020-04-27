using System.Collections;
using UnityEngine;
using System.Numerics;

public class CPUFractalController : MonoBehaviour
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
    private Coroutine drawingThread;
    double viewPortX, viewPortY;
    void Awake()
    {   
        finished = false;
        //results = new Color[pwidth, pheight];
        //resultArray = new NativeArray<Color>(pwidth * pheight, Allocator.Persistent);
        double ratio = pwidth / pheight;
        if (ratio > 1f){
            xmax *= ratio;
        }else{
            ymax /= ratio;
        }
        xmin = -xmax;
        ymin = -ymax;
        brush = new Texture2D((int)pwidth, (int)pheight);
        viewPortWidth = xmax - xmin;
        viewPortHeight = ymax - ymin;
        //ChangeTheColor();
        //ChangeTheColorParallel();
    }

    void Start(){
        drawingThread = StartCoroutine(ChangeTheColor());
        finished = true;
    }

    public void StartDraw(){
        try{
            StopCoroutine(drawingThread);
        }catch{}
        finished = false;
        drawingThread =  StartCoroutine(ChangeTheColor());
    }

    public void StopDrawingCorroutine(){
        try{
            StopCoroutine(drawingThread);
            Debug.Log("Mandelbrot drawing corroutine finished successfully.");
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

    public double GetViewPortX(int x){
        return xmin + ((double)x / pwidth) * viewPortWidth + panX;
    }

    public double GetViewPortY(int y){
        return ymin + ((double)y / pheight) * viewPortHeight + panY;
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

    IEnumerator ChangeTheColor(){
        yield return null;
        int i;
        int x, y;
        finished = false;
        count = 0;
        xmin = -xmax;
        ymin = -ymax;
        brush = new Texture2D((int)pwidth, (int)pheight);
        viewPortWidth = xmax - xmin;
        viewPortHeight = ymax - ymin;
        for (x = 0; x < pwidth; x++){
            for (y = 0; y < pheight; y++){
                Color value;
                
                viewPortX = xmin + ((double)x / pwidth) * viewPortWidth + panX;
                viewPortY = ymin + ((double)y / pheight) * viewPortHeight + panY;
                Complex z = new Complex(0.0f, 0.0f);
                Complex c = new Complex(viewPortX, viewPortY);
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
            if (x % 10 == 0) brush.Apply(); yield return new WaitForEndOfFrame();
        }
        brush.Apply();
        yield return new WaitForSeconds(0.5f);
        finished = true;
        
    }
        
    void OnGUI()
    {
        //GUILayout.Label(brush);
        //GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, pwidth, pheight), brush);
        GUI.DrawTexture(new Rect(Screen.width - positionX, Screen.height-positionY, pwidth, pheight), brush);
    }
}
