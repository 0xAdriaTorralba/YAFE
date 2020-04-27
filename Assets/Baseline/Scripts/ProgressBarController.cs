using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject fractalRenderer;
    public CPUFractalController fractalMandelbrot;

    public CPUJuliaController fractalJulia;
    private Image progressBar;
    public string attatch;

    private Text percentage;
    void Start()
    {
        fractalRenderer = GameObject.FindGameObjectWithTag(attatch);
        progressBar = this.transform.Find("Bar").GetComponent<Image>();
        percentage = this.transform.Find("Percentage").GetComponent<Text>();
        if (attatch.Equals("Mandelbrot")){ 
            fractalMandelbrot = fractalRenderer.GetComponent<CPUFractalController>();
            StartCoroutine(UpdateProgressBarMandelbrot(fractalMandelbrot));
        }else{
            fractalJulia = fractalRenderer.GetComponent<CPUJuliaController>();
            StartCoroutine(UpdateProgressBarJulia(fractalJulia));
        } 
        
        
        //StartCoroutine(UpdateProgressBarMandelbrot(fractalController));
    }

    public void StartProgressBarMandelbrot(){
        StartCoroutine(UpdateProgressBarMandelbrot(fractalMandelbrot));
    }

    public void StartProgressBarJulia(){
        StartCoroutine(UpdateProgressBarJulia(fractalJulia));
    }

    // Update is called once per frame
    void Update()
    {
        //progressBar.fillAmount = fractalController.GetProgress();
    }

    IEnumerator UpdateProgressBarMandelbrot(CPUFractalController fractalController){
        while(!fractalController.GetFinished()){
            progressBar.fillAmount = (float)fractalController.GetProgress();
            percentage.text = fractalController.GetProgress() * 100 + " %";
            yield return null;
        }
    }

    IEnumerator UpdateProgressBarJulia(CPUJuliaController fractalController){
        while(!fractalController.GetFinished()){
            progressBar.fillAmount = (float)fractalController.GetProgress();
            percentage.text = fractalController.GetProgress() * 100 + " %";
            yield return null;
        }
    }
}
