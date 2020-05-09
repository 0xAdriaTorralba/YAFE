using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;



[System.Serializable]
public class RenderingParameters{
    public Texture2D tex2D;
    public Image fractalImage;
    public int pwidth = 750;
    public int pheight = 750;
    public double xmin, xmax = 2.0, ymin, ymax = 2.0;
    public double panX, panY;
    public double viewPortX, viewPortY;
    public double viewPortWidth, viewPortHeight;
    public int count;
    public bool finished;
    public Coroutine drawingThread;
}
[System.Serializable]
public class FractalParameters{
    public int maxIters = 100;
    public string algorithm;
    public string colorMap;
}

public abstract class Fractal : MonoBehaviour
{

    public RenderingParameters rp;
    public FractalParameters fp;


    void Awake(){
        
        rp.fractalImage = GetComponent<Image>();
        rp.tex2D = new Texture2D((int) rp.pwidth, (int) rp.pheight);
    }

    void Start(){
        
    }

    void Update(){
        
    }

    public abstract void StartDraw();

    public abstract void OnEnable();

    public float GetProgress(){
        return (float) rp.count / (rp.pwidth * rp.pheight);
    }

    public bool GetFinished(){
        return rp.finished;
    }

    public double GetViewPortX(double x){
        return rp.xmin + ((double) x / rp.pwidth) * rp.viewPortWidth + rp.panX;
    }

    public double GetViewPortY(double y){
        return rp.ymin + ((double) y / rp.pheight) * rp.viewPortHeight + rp.panY;
    }

    protected void CorrectAspectRatio(){
        double ratio = rp.pwidth / rp.pheight;
        if (ratio > 1.0){
            rp.xmax *= ratio;
        }else{
            rp.ymax /= ratio;
        }
    }

    protected IEnumerator Draw(){
        yield return null;
        throw new Exception("ERROR! You need to implement your own draw function!");
    }
}
