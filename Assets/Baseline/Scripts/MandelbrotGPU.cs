using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MandelbrotGPU : FractalGPU
{

    void Start(){
        UpdateZoom(2.0f);
        UpdatePosition(0.0f, 0.0f);
        UpdateColormap("Colormap 1 (Sin)");
        UpdateIterations(100);
        UpdateThreshold(2);
        UpdateDegree(2);
        RedrawAll();
    }
 

}
