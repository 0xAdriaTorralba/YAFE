using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;




public abstract class FractalCPU : Fractal
{

    public class ColorData{
        public Color color;
        public int x;
        public int y;

        public ColorData(Color color, int x, int y){
            this.color = color;
            this.x = x;
            this.y = y;
        }
    }

    public abstract void StartDraw();

    public abstract void OnEnable();

    public float GetProgress(){
        return (float) rp.count / (rp.pwidth * rp.pheight);
    }

    public void ResetRenderParameters(){
        rp.xmax = 2.0;
        rp.ymax = 2.0;
        rp.panX = 0.0;
        rp.panY = 0.0;

    }

    protected IEnumerator Draw(){
        yield return null;
        throw new Exception("ERROR! You need to implement your own draw function!");
    }
}
