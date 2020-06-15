using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FractalGPU : Fractal
{
    protected RawImage image;
    protected Material material;


    void Awake(){
        // This script should be ALWAYS attached to a GO with an image.
        image = GetComponent<RawImage>();
        material = image.materialForRendering;
        rp.tex2D = new Texture2D(rp.pwidth, rp.pheight);
        //image.sprite = Sprite.Create(rp.tex2D, new Rect(0, 0, rp.tex2D.width, rp.tex2D.height), new UnityEngine.Vector2(0.5f, 0.5f)); 
        this.rp.finished = true;
    }


    void Update(){
        this.rp.viewPortWidth = this.rp.xmax - this.rp.xmin;
        this.rp.viewPortHeight = this.rp.ymax - this.rp.ymin;
    }


    // BEGIN OF PROTECTED METHODS

    protected void RedrawZoom(){
        material.SetColor("_Zoom", new Vector4((float)rp.xmax, (float)rp.ymax, 0.0f, 0.0f));
        image.material = material;
    }

    protected void RedrawPosition(){
        material.SetColor("_Pan", new Vector4((float)rp.panX, (float)rp.panY, 0.0f, 0.0f));
        image.material = material;
    }

    protected void RedrawColormap(){
        switch (fp.colorMap){
            case "Colormap 1 (Sin)":
                material.SetInt("_Colormap", 1);
                image.material = material;
                break;
            case "Colormap 2 (Cos)":
                material.SetInt("_Colormap", 2);
                image.material = material;
                break;
            case "Colormap 3 (Tan)":
                material.SetInt("_Colormap", 3);
                image.material = material;
                break;
            default:
                material.SetInt("_Colormap", 1);
                image.material = material;
                break;

        }

    }

    protected void RedrawAlgorithm(){
        if (this is MandelbrotGPU){
            switch (fp.algorithm){
                case "Escape Algorithm":
                    material.shader = Shader.Find("FractalShaders/MandelbrotShader");
                    image.material = material;
                    break;
                case "Henriksen Algorithm":
                    material.shader = Shader.Find("FractalShaders/MandelbrotHenriksenShader");
                    image.material = material;
                    break;
                default:
                    material.shader = Shader.Find("FractalShaders/MandelbrotShader");
                    image.material = material;
                    break;
            }
        }
        if (this is JuliaGPU){
            switch (fp.algorithm){
                case "Escape Algorithm":
                    material.shader = Shader.Find("FractalShaders/JuliaShader");
                    image.material = material;
                    break;
                case "Henriksen Algorithm":
                    material.shader = Shader.Find("FractalShaders/JuliaHenriksenShader");
                    image.material = material;
                    break;
                default:
                    material.shader = Shader.Find("FractalShaders/JuliaShader");
                    image.material = material;
                    break;
            }
        }            
        
    }

    protected void RedrawThreshold(){
        material.SetInt("_Threshold", this.fp.threshold);
        image.material = material;
    }

    protected void RedrawIterations(){
        material.SetInt("_Iterations", this.fp.maxIters);
        image.material = material;
    }


    protected void RedrawDegree(){
        material.SetInt("_Degree", this.fp.degree);
        image.material = material;
    }

    protected void RedrawDetail(){
        material.SetInt("_Detail", this.fp.detail);
        image.material = material;
    }


    protected void RedrawAll(){
        RedrawZoom();
        RedrawPosition();
        RedrawColormap();
        RedrawThreshold();
        RedrawIterations();
    }

    // END OF PROTECTED METHODS
    public void UpdateIterations(int maxIterations){
        this.fp.maxIters = maxIterations;
        RedrawIterations();
    }

    public void UpdateColormap(string colormap){
        this.fp.colorMap = colormap;
        RedrawColormap();
    }

    public void UpdateAlgorithm(string algorihm){
        this.fp.algorithm = algorihm;
        RedrawAlgorithm();
    }
    public void UpdateDegree(int degree){
        this.fp.degree = degree;
        RedrawDegree();
    }

    public void UpdateDetail(int detail){
        this.fp.detail = detail;
        RedrawDetail();
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

    public void UpdateThreshold(int threshold){
        this.fp.threshold = threshold;
        RedrawThreshold();
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

    // Due to some shaders stuff, it is needed to substract (-) the pan instead of add them to correctly
    // visualise the coordinates.
    public new double GetViewPortX(double x){
        return rp.xmin + ((double) x / rp.pwidth) * rp.viewPortWidth - rp.panX;
    }

    public new double GetViewPortY(double y){
        return rp.ymin + ((double) y / rp.pheight) * rp.viewPortHeight - rp.panY;
    }

}
