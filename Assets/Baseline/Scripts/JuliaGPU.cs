using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class JuliaGPU : Fractal
{

    private Image image;
    private CameraMovementController cameraController;
    private Material material;

    private float reZ, imZ;

    void Awake(){
        image = GetComponent<Image>();
        material = image.material;
    }

    void Start(){
        this.fp.colorMap = "Colormap 1 (Sin)";
        this.fp.maxIters = 100;
        this.rp.panX = 0.0;
        this.rp.panY = 0.0;
        this.rp.xmax = 2.0;
        this.rp.xmin = -2.0;
        this.rp.ymax = 2.0;
        this.rp.ymin = -2.0;
        this.rp.viewPortWidth = this.rp.xmax - this.rp.xmin;
        this.rp.viewPortHeight = this.rp.ymax - this.rp.ymin;
        RedrawAll();
    }

    void Update(){

    }



    public override void StartDraw(){

    }

    public override void OnEnable(){
        RedrawAll();
    }

    private void RedrawAll(){
        RedrawZoom();
        RedrawPosition();
        RedrawSeed();
        RedrawThreshold();
        RedrawIterations();
    }
    

    private void RedrawColormap(){
    switch (fp.colorMap){
        case "Colormap 1 (Sin)":
            material.SetInt("_Colormap", 1);
            break;
        case "Colormap 2 (Cos)":
            material.SetInt("_Colormap", 2);
            break;
        case "Colormap 3 (Tan)":
            material.SetInt("_Colormap", 3);
            break;
        default:
            material.SetInt("_Colormap", 1);
            break;

    }
    image.material = material;

    }

    private void RedrawZoom(){
        material.SetColor("_Zoom", new Vector4((float)rp.xmax, (float)rp.ymax, 0.0f, 0.0f));
        image.material = material;
    }

    private void RedrawPosition(){
        material.SetColor("_Pan", new Vector4((float)rp.panX, (float)rp.panY, 0.0f, 0.0f));
        image.material = material;
    }

    private void RedrawSeed(){
        material.SetColor("_Seed", new Vector4(reZ, imZ, 0.0f, 0.0f));
        image.material = material;
    }

    private void RedrawAlgorithm(){
        material.SetInt("_Algorithm", 1);
        image.material = material;
    }

    private void RedrawThreshold(){
        material.SetInt("_Threshold", this.fp.threshold);
        image.material = material;
    }

    private void RedrawIterations(){
        material.SetInt("_Iterations", this.fp.maxIters);
        image.material = material;
    }



    public void UpdateColormap(string colormap){
        this.fp.colorMap = colormap;
        RedrawColormap();
    }

    public void UpdateSeed(float reZ, float imZ){
        UpdateZoom(2.0f);
        UpdatePosition(0.0f, 0.0f);
        this.reZ = reZ;
        this.imZ = imZ;
        RedrawSeed();
    }

    public void UpdateIterations(int maxIterations){
        this.fp.maxIters = maxIterations;
        material.SetFloat("_Iterations", maxIterations);
        RedrawAll();
    }

    public void UpdateThreshold(int threshold){
        this.fp.threshold = threshold;
        material.SetFloat("_Threshold", threshold);
        RedrawAll();
    }

    public void UpdateZoom(float zoom){
        this.rp.xmax = zoom;
        this.rp.xmin = -zoom;
        this.rp.ymax = zoom;
        this.rp.ymin = -zoom;
        this.rp.viewPortWidth = this.rp.xmax - this.rp.xmin;
        this.rp.viewPortHeight = this.rp.ymax - this.rp.ymin;
        RedrawZoom();

    }

    public void UpdatePosition(float panX, float panY){
        UpdatePanX(panX);
        UpdatePanY(panY);
        RedrawPosition();

    }

    private void UpdatePanX(float panX){
        this.rp.panX = panX;
    }

    private void UpdatePanY(float panY){
        this.rp.panY = panY;
    }

}
