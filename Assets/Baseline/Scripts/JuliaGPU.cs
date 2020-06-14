using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class JuliaGPU : FractalGPU
{

    private float reZ, imZ;


    void Start(){
        UpdateZoom(0.25f);
        UpdatePosition(0.4986667f, -0.05466667f);
        UpdateColormap("Colormap 1 (Sin)");
        UpdateIterations(100);
        UpdateThreshold(2);
        UpdateSeed(-0.765f, 0.12f);
        UpdateDegree(2);
        this.RedrawAll();
    }

    protected new void RedrawAll(){
        base.RedrawAll();
        RedrawSeed();
    }

    private void RedrawSeed(){
        material.SetColor("_Seed", new Vector4(reZ, imZ, 0.0f, 0.0f));
        image.material = material;
    }

    public void UpdateSeed(float reZ, float imZ){
        //UpdateZoom(2.0f);
        //UpdatePosition(0.0f, 0.0f);
        this.reZ = reZ;
        this.imZ = imZ;
        RedrawSeed();
    }

}
