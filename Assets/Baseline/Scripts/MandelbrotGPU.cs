using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MandelbrotGPU : FractalGPU
{

    void Start(){
        UpdateZoom(0.00012207031250f);
        UpdatePosition(1.25360091145834000000f, -0.38446614583333f);
        UpdateColormap("Colormap 1 (Sin)");
        UpdateIterations(100);
        UpdateThreshold(2);
        UpdateDegree(2);
        RedrawAll();
    }
 

}
