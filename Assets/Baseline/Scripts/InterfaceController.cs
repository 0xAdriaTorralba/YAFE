using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InterfaceController : MonoBehaviour
{
    // Start is called before the first frame update

    private Text realPart, imaginaryPart;

    private Text realPartJulia, imaginaryPartJulia;
    private CPUFractalController fractalMandelbrot;
    private CPUJuliaController fractalJulia;
    private ProgressBarController progressBarControllerMandelbrot, progressBarControllerJulia;
    private GameObject eventSystem;

    private Button zoomInMandelbrot, zoomOutMandelbrot, refreshMandelbrot, upMandelbrot, downMandelbrot, leftMandelbrot, rightMandelbrot;

    private Button zoomInJulia, zoomOutJulia, refreshJulia, upJulia, downJulia, leftJulia, rightJulia;

    private double rez, imz;

    private GameObject dialogBox;

    private double defaultZoom, defaultMovement;
    void Awake()
    {
        dialogBox = GameObject.FindGameObjectWithTag("DialogBox");
        dialogBox.SetActive(false);

        GameObject complexNumber = GameObject.Find("Complex Number Mandelbrot/Real Part");
        realPart = complexNumber.GetComponent<Text>();
        realPartJulia = GameObject.Find("Complex Number Julia/Real Part").GetComponent<Text>();
        complexNumber = GameObject.Find("Complex Number Mandelbrot/Imaginary Part");
        imaginaryPart = complexNumber.GetComponent<Text>();
        imaginaryPartJulia = GameObject.Find("Complex Number Julia/Imaginary Part").GetComponent<Text>();
        fractalMandelbrot = GameObject.FindGameObjectWithTag("Mandelbrot").GetComponent<CPUFractalController>();
        fractalJulia = GameObject.FindGameObjectWithTag("Julia").GetComponent<CPUJuliaController>();
        progressBarControllerJulia = GameObject.Find("Progress Bar Julia").GetComponent<ProgressBarController>();
        progressBarControllerMandelbrot = GameObject.Find("Progress Bar Mandelbrot").GetComponent<ProgressBarController>();
        eventSystem = GameObject.FindGameObjectWithTag("EventSystem");

        // Buttons setup

        zoomInMandelbrot = GameObject.Find("Utilities Mandelbrot/Zoom In").GetComponent<Button>();
        zoomOutMandelbrot = GameObject.Find("Utilities Mandelbrot/Zoom Out").GetComponent<Button>();
        refreshMandelbrot = GameObject.Find("Utilities Mandelbrot/Refresh").GetComponent<Button>();
        upMandelbrot = GameObject.Find("Utilities Mandelbrot/Movement Arrows/Up").GetComponent<Button>();
        downMandelbrot = GameObject.Find("Utilities Mandelbrot/Movement Arrows/Down").GetComponent<Button>();
        leftMandelbrot = GameObject.Find("Utilities Mandelbrot/Movement Arrows/Left").GetComponent<Button>();
        rightMandelbrot = GameObject.Find("Utilities Mandelbrot/Movement Arrows/Right").GetComponent<Button>();

        zoomInJulia = GameObject.Find("Utilities Julia/Zoom In").GetComponent<Button>();
        zoomOutJulia = GameObject.Find("Utilities Julia/Zoom Out").GetComponent<Button>();
        refreshJulia = GameObject.Find("Utilities Julia/Refresh").GetComponent<Button>();
        upJulia = GameObject.Find("Utilities Julia/Movement Arrows/Up").GetComponent<Button>();
        downJulia = GameObject.Find("Utilities Julia/Movement Arrows/Down").GetComponent<Button>();
        leftJulia = GameObject.Find("Utilities Julia/Movement Arrows/Left").GetComponent<Button>();
        rightJulia = GameObject.Find("Utilities Julia/Movement Arrows/Right").GetComponent<Button>();

        defaultZoom = 0.5;
        defaultMovement = 0.1;

    }

    void Start(){

        zoomInMandelbrot.onClick.AddListener(() => ZoomInMandelbrot(defaultZoom));
        zoomOutMandelbrot.onClick.AddListener(() => ZoomOutMandelbrot(defaultZoom));
        upMandelbrot.onClick.AddListener(() => UpMandelbrot(defaultMovement));
        downMandelbrot.onClick.AddListener(() => DownMandelbrot(defaultMovement));
        leftMandelbrot.onClick.AddListener(() => LeftMandelbrot(defaultMovement));
        rightMandelbrot.onClick.AddListener(() => RightMandelbrot(defaultMovement));
        refreshMandelbrot.onClick.AddListener(() => RefreshFractalMandelbrot());



        zoomInJulia.onClick.AddListener(() => ZoomInJulia(defaultZoom));
        zoomOutJulia.onClick.AddListener(() => ZoomOutJulia(defaultZoom));
        upJulia.onClick.AddListener(() => UpJulia(defaultMovement));
        downJulia.onClick.AddListener(() => DownJulia(defaultMovement));
        leftJulia.onClick.AddListener(() => LeftJulia(defaultMovement));
        rightJulia.onClick.AddListener(() => RightJulia(defaultMovement));
        refreshJulia.onClick.AddListener(() => RefreshFractalJulia());
        
        
        
        StartCoroutine(ListenerFractal());

    }

    private void ZoomInMandelbrot(double defaultZoom){
        fractalMandelbrot.xmax *= defaultZoom;
        fractalMandelbrot.ymax *= defaultZoom;

        //fractalMandelbrot.xmax = Mathf.Max((float)fractalMandelbrot.xmax, 1e-14f);
        //fractalMandelbrot.ymax = Mathf.Max((float)fractalMandelbrot.ymax, 1e-14f);

        RefreshFractalMandelbrot();
    }

    private void ZoomOutMandelbrot(double defaultZoom){
        fractalMandelbrot.xmax /= defaultZoom;
        fractalMandelbrot.ymax /= defaultZoom;

        //fractalMandelbrot.xmax = Mathf.Min((float)fractalMandelbrot.xmax, 2);
        //fractalMandelbrot.ymax = Mathf.Min((float)fractalMandelbrot.ymax, 2);

        RefreshFractalMandelbrot();
    }

    private void UpMandelbrot(double defaultMovement){
        fractalMandelbrot.panY += defaultMovement;

        //fractalMandelbrot.panY = Mathf.Min((float) fractalMandelbrot.panY, 2);

        RefreshFractalMandelbrot();
    }

    private void DownMandelbrot(double defaultMovement){
        fractalMandelbrot.panY -= defaultMovement;

        //fractalMandelbrot.panY = Mathf.Max((float) fractalMandelbrot.panY, -2);

        RefreshFractalMandelbrot();
    }

    private void LeftMandelbrot(double defaultMovement){
        fractalMandelbrot.panX -= defaultMovement;

        //fractalMandelbrot.panX = Mathf.Max((float) fractalMandelbrot.panX, -2);

        RefreshFractalMandelbrot();
    }

    private void RightMandelbrot(double defaultMovement){
        fractalMandelbrot.panX += defaultMovement;

        //fractalMandelbrot.panX = Mathf.Min((float) fractalMandelbrot.panX, 2);

        RefreshFractalMandelbrot();
    }

    private void RefreshFractalMandelbrot(){
        fractalMandelbrot.StopDrawingCorroutine();
        fractalMandelbrot.StartDraw();
        progressBarControllerMandelbrot.StartProgressBarMandelbrot();
    }

    private void ZoomInJulia(double defaultZoom){
        fractalJulia.xmax *= defaultZoom;
        fractalJulia.ymax *= defaultZoom;

        //fractalJulia.xmax = Mathf.Max((float)fractalJulia.xmax, 1e-14f);
        //fractalJulia.ymax = Mathf.Max((float)fractalJulia.ymax, 1e-14f);

        RefreshFractalJulia();
    }

    private void ZoomOutJulia(double defaultZoom){
        fractalJulia.xmax /= defaultZoom;
        fractalJulia.ymax /= defaultZoom;

        //fractalJulia.xmax = Mathf.Min((float)fractalJulia.xmax, 2);
        //fractalJulia.ymax = Mathf.Min((float)fractalJulia.ymax, 2);

        RefreshFractalJulia();
    }

       private void UpJulia(double defaultMovement){
        fractalJulia.panY += defaultMovement;

        //fractalJulia.panY = Mathf.Min((float) fractalJulia.panY, 2);

        RefreshFractalJulia();
    }

    private void DownJulia(double defaultMovement){
        fractalJulia.panY -= defaultMovement;

        //fractalJulia.panY = Mathf.Max((float) fractalJulia.panY, -2);

        RefreshFractalJulia();
    }

    private void LeftJulia(double defaultMovement){
        fractalJulia.panX -= defaultMovement;

        
        //fractalJulia.panX = Mathf.Max((float) fractalJulia.panX, -2);

        RefreshFractalJulia();
    }

    private void RightJulia(double defaultMovement){
        fractalJulia.panX += defaultMovement;

        //fractalJulia.panX = Mathf.Min((float) fractalJulia.panX, 2);

        RefreshFractalJulia();
    }


    

    private void RefreshFractalJulia(){
        fractalJulia.StopDrawingCorroutine();
        fractalJulia.RedrawCurrent();
        progressBarControllerJulia.StartProgressBarJulia();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            if (   (int) Input.mousePosition.x - (Screen.width - fractalMandelbrot.positionX) >= 0 
                && (int) Input.mousePosition.x - (Screen.width - fractalMandelbrot.positionX) <= fractalMandelbrot.pwidth
                && (int) Input.mousePosition.y - 25 - (Screen.height - fractalMandelbrot.positionY) >= 0 
                && (int) Input.mousePosition.y - 25 - (Screen.height - fractalMandelbrot.positionY) <= fractalMandelbrot.pheight){
                    fractalJulia.StartDraw(rez, imz);
                    //realPartJulia.text = "z = " + rez;
                    //imaginaryPartJulia.text = "+ i " + imz;
                    if (imz < 0) realPartJulia.text = "c = "+ rez + " - i "+ Math.Abs(imz);
                    else realPartJulia.text = "c = "+ rez + " + i "+ imz;
                    progressBarControllerJulia.StartProgressBarJulia();
                }
        }
    }

    IEnumerator ListenerFractal(){
        while (true){
            if (   ((int) Input.mousePosition.x - (Screen.width - fractalMandelbrot.positionX) >= 0) 
                && ((int) Input.mousePosition.x - (Screen.width - fractalMandelbrot.positionX) <= fractalMandelbrot.pwidth)
                && ((int) Input.mousePosition.y - 25 - (Screen.height - fractalMandelbrot.positionY) >= 0) 
                && ((int) Input.mousePosition.y - 25 - (Screen.height - fractalMandelbrot.positionY) <= fractalMandelbrot.pheight)){
                    rez = fractalMandelbrot.GetViewPortX((int)Input.mousePosition.x - (Screen.width - fractalMandelbrot.positionX));
                    imz = fractalMandelbrot.GetViewPortY((int)Input.mousePosition.y - 25 - (Screen.height - fractalMandelbrot.positionY));
                    //realPart.text = "z = "+ rez;
                    //imaginaryPart.text = "+ i "+ imz;
                    if (imz < 0) realPart.text = "z = "+ rez + " - i "+ Math.Abs(imz);
                    else realPart.text = "z = "+ rez + " + i "+ imz;
                    
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void StopAllDrawingCoroutines(){
        fractalMandelbrot.StopDrawingCorroutine();
        fractalJulia.StopDrawingCorroutine();
    }

    public void StartDrawing(){
        fractalMandelbrot.StartDraw();
        progressBarControllerMandelbrot.StartProgressBarMandelbrot();
    }

    public void RestartDrawingCoroutines(){
        StopAllDrawingCoroutines();
        StartDrawing();
    }

    public void ToggleAboutDialog(){
        dialogBox.SetActive(!dialogBox.activeInHierarchy);
    }
}
