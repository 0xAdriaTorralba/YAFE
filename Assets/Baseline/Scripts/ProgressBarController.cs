﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBarController : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject fractalRenderer;
    public CPUFractalController fractalMandelbrot;

    public CPUJuliaController fractalJulia;
    private Image progressBar;
    public string attatch;

    private TextMeshProUGUI percentage;

    private Coroutine pbMandelbrot = null, pbJulia = null;
    void Start()
    {
        fractalRenderer = GameObject.FindGameObjectWithTag(attatch);
        progressBar = this.transform.Find("Bar").GetComponent<Image>();
        percentage = this.transform.Find("Percentage").GetComponent<TextMeshProUGUI>();
        if (attatch.Equals("Mandelbrot")){ 
            fractalMandelbrot = fractalRenderer.GetComponent<CPUFractalController>();
            pbMandelbrot = StartCoroutine(UpdateProgressBarMandelbrot(fractalMandelbrot));
        }else{
            fractalJulia = fractalRenderer.GetComponent<CPUJuliaController>();
            pbJulia = StartCoroutine(UpdateProgressBarJulia(fractalJulia));
        } 
        
    }

    void Update(){
        if (attatch.Equals("Mandelbrot")){
            if (!fractalMandelbrot.GetFinished() && pbMandelbrot != null){
                StartProgressBarMandelbrot();
            }
        }
        if (attatch.Equals("Julia")){
            if (!fractalJulia.GetFinished() && pbJulia != null){
                StartProgressBarJulia();
            }
        }
    }

    public void StartProgressBarMandelbrot(){
        pbMandelbrot = StartCoroutine(UpdateProgressBarMandelbrot(fractalMandelbrot));
    }

    public void StartProgressBarJulia(){
        pbJulia = StartCoroutine(UpdateProgressBarJulia(fractalJulia));
    }

    IEnumerator UpdateProgressBarMandelbrot(CPUFractalController fractalController){
        while(!fractalController.GetFinished()){
            progressBar.fillAmount = (float)fractalController.GetProgress();
            percentage.text = (int)(fractalController.GetProgress() * 100)+ "";
            yield return null;
        }
    }

    IEnumerator UpdateProgressBarJulia(CPUJuliaController fractalController){
        while(!fractalController.GetFinished()){
            progressBar.fillAmount = (float)fractalController.GetProgress();
            percentage.text = (int)(fractalController.GetProgress() * 100)+ "";
            yield return null;
        }
    }
}