using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using TMPro;

public class InterfaceController : MonoBehaviour
{
    // Start is called before the first frame update

    private TextMeshProUGUI mandelbrotNumber;

    private Mandelbrot fractalMandelbrot;
    private Julia fractalJulia;
    private ProgressBarController progressBarControllerMandelbrot, progressBarControllerJulia;

    private CoordinatesListener coordinatesListener;
    private GameObject eventSystem;

    private Button zoomInMandelbrot, zoomOutMandelbrot, refreshMandelbrot, upMandelbrot, downMandelbrot, leftMandelbrot, rightMandelbrot;

    private Button zoomInJulia, zoomOutJulia, refreshJulia, upJulia, downJulia, leftJulia, rightJulia;

    private TMP_InputField textZoomM, textPanXM, textPanYM;
    private TMP_InputField textZoomJ, textPanXJ, textPanYJ;

    private TMP_InputField realPartJulia, imaginaryPartJulia;

    private const string format = "F10";

    private double rez, imz;

    private List<TMP_InputField> inputFields, inputFieldsJulia;

    private List<string> previousValues, previousValuesJulia;
    
    private bool allowEnter, allowEnterJulia;


    private double defaultZoom, defaultMovement;
    void Awake()
    {
        
        // Finds
        mandelbrotNumber = GameObject.Find("Complex Number Mandelbrot/Mandelbrot Text").GetComponent<TextMeshProUGUI>();
        //juliaNumber = GameObject.Find("Complex Number Julia/Julia Text").GetComponent<TextMeshProUGUI>();


        fractalMandelbrot = GameObject.FindGameObjectWithTag("Mandelbrot").GetComponent<Mandelbrot>();
        fractalJulia = GameObject.FindGameObjectWithTag("Julia").GetComponent<Julia>();


        progressBarControllerJulia = GameObject.Find("Progress Bar Julia").GetComponent<ProgressBarController>();
        progressBarControllerMandelbrot = GameObject.Find("Progress Bar Mandelbrot").GetComponent<ProgressBarController>();


        coordinatesListener = GameObject.FindGameObjectWithTag("Mandelbrot").GetComponent<CoordinatesListener>();


        textZoomM = GameObject.Find("Utilities Mandelbrot/Text Inputs/Zoom Input Field").GetComponent<TMP_InputField>();
        textPanXM = GameObject.Find("Utilities Mandelbrot/Text Inputs/Pan X Input Field").GetComponent<TMP_InputField>();
        textPanYM = GameObject.Find("Utilities Mandelbrot/Text Inputs/Pan Y Input Field").GetComponent<TMP_InputField>();

        textZoomJ = GameObject.Find("Utilities Julia/Text Inputs/Zoom Input Field").GetComponent<TMP_InputField>();
        textPanXJ = GameObject.Find("Utilities Julia/Text Inputs/Pan X Input Field").GetComponent<TMP_InputField>();
        textPanYJ = GameObject.Find("Utilities Julia/Text Inputs/Pan Y Input Field").GetComponent<TMP_InputField>();

        realPartJulia = GameObject.Find("Complex Number Julia/Input Real Julia").GetComponent<TMP_InputField>();
        imaginaryPartJulia = GameObject.Find("Complex Number Julia/Input Imaginary Julia").GetComponent<TMP_InputField>();

        inputFields = new List<TMP_InputField>();
        inputFieldsJulia = new List<TMP_InputField>();
        previousValues = new List<string>();
        previousValuesJulia = new List<string>();

        inputFields.Add(textZoomM);
        inputFields.Add(textPanXM);
        inputFields.Add(textPanYM);
        
        inputFields.Add(textZoomJ);
        inputFields.Add(textPanXJ);
        inputFields.Add(textPanYJ);

        inputFieldsJulia.Add(realPartJulia);
        inputFieldsJulia.Add(imaginaryPartJulia);

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
        
        textZoomM.text = fractalMandelbrot.rp.xmax.ToString(format);
        textZoomJ.text = fractalJulia.rp.xmax.ToString(format);

        textPanXM.text = fractalMandelbrot.rp.panX.ToString(format);
        textPanXJ.text = fractalJulia.rp.panX.ToString(format);

        textPanYM.text = fractalMandelbrot.rp.panY.ToString(format);
        textPanYJ.text = fractalJulia.rp.panY.ToString(format);

        previousValues.Add(textZoomM.text);
        previousValues.Add(textPanXM.text);
        previousValues.Add(textPanYM.text);

        previousValues.Add(textZoomJ.text);
        previousValues.Add(textPanXJ.text);
        previousValues.Add(textPanYJ.text);

        previousValuesJulia.Add("0.0");
        previousValuesJulia.Add("0.0");
        
        allowEnter = false;
        allowEnterJulia = false;

        

    }

    void OnEnable(){
        StartCoroutine(OnEnableCoroutine());
    }

    void OnDisable(){
        OnSubmit();
        OnSubmitJulia();
    }

    private IEnumerator OnEnableCoroutine(){
        yield return new WaitForSeconds(0.1f);
        RefreshFractalMandelbrot();
        RefreshFractalJulia();
        StartCoroutine(ListenerFractal());
    }

    private void ZoomInMandelbrot(double defaultZoom){
        fractalMandelbrot.rp.xmax *= defaultZoom;
        fractalMandelbrot.rp.ymax *= defaultZoom;

        textZoomM.text = fractalMandelbrot.rp.xmax.ToString(format);

        //fractalMandelbrot.xmax = Mathf.Max((float)fractalMandelbrot.xmax, 1e-14f);
        //fractalMandelbrot.ymax = Mathf.Max((float)fractalMandelbrot.ymax, 1e-14f);

        RefreshFractalMandelbrot();
    }

    private void ZoomOutMandelbrot(double defaultZoom){
        fractalMandelbrot.rp.xmax /= defaultZoom;
        fractalMandelbrot.rp.ymax /= defaultZoom;

        textZoomM.text = fractalMandelbrot.rp.xmax.ToString(format);

        //fractalMandelbrot.xmax = Mathf.Min((float)fractalMandelbrot.xmax, 2);
        //fractalMandelbrot.ymax = Mathf.Min((float)fractalMandelbrot.ymax, 2);

        RefreshFractalMandelbrot();
    }

    private void UpMandelbrot(double defaultMovement){
        fractalMandelbrot.rp.panY += defaultMovement;

        textPanYM.text = fractalMandelbrot.rp.panY.ToString(format);

        //fractalMandelbrot.panY = Mathf.Min((float) fractalMandelbrot.panY, 2);

        RefreshFractalMandelbrot();
    }

    private void DownMandelbrot(double defaultMovement){
        fractalMandelbrot.rp.panY -= defaultMovement;

        textPanYM.text = fractalMandelbrot.rp.panY.ToString(format);

        //fractalMandelbrot.panY = Mathf.Max((float) fractalMandelbrot.panY, -2);

        RefreshFractalMandelbrot();
    }

    private void LeftMandelbrot(double defaultMovement){
        fractalMandelbrot.rp.panX -= defaultMovement;

        textPanXM.text = fractalMandelbrot.rp.panX.ToString(format);

        //fractalMandelbrot.panX = Mathf.Max((float) fractalMandelbrot.panX, -2);

        RefreshFractalMandelbrot();
    }

    private void RightMandelbrot(double defaultMovement){
        fractalMandelbrot.rp.panX += defaultMovement;

        textPanXM.text = fractalMandelbrot.rp.panX.ToString(format);


        //fractalMandelbrot.panX = Mathf.Min((float) fractalMandelbrot.panX, 2);

        RefreshFractalMandelbrot();
    }

    public void RefreshFractalMandelbrot(){
        UpdateMandelbrotRenderingValues();
        fractalMandelbrot.StartDraw();
        progressBarControllerMandelbrot.StartProgressBarMandelbrot();
    }

    private void ZoomInJulia(double defaultZoom){
        fractalJulia.rp.xmax *= defaultZoom;
        fractalJulia.rp.ymax *= defaultZoom;

        textZoomJ.text = fractalJulia.rp.xmax.ToString(format);

        //fractalJulia.xmax = Mathf.Max((float)fractalJulia.xmax, 1e-14f);
        //fractalJulia.ymax = Mathf.Max((float)fractalJulia.ymax, 1e-14f);

        RefreshFractalJulia();
    }

    private void ZoomOutJulia(double defaultZoom){
        fractalJulia.rp.xmax /= defaultZoom;
        fractalJulia.rp.ymax /= defaultZoom;

        textZoomJ.text = fractalJulia.rp.xmax.ToString(format);


        //fractalJulia.xmax = Mathf.Min((float)fractalJulia.xmax, 2);
        //fractalJulia.ymax = Mathf.Min((float)fractalJulia.ymax, 2);

        RefreshFractalJulia();
    }

       private void UpJulia(double defaultMovement){
        fractalJulia.rp.panY += defaultMovement;

        textPanYJ.text = fractalJulia.rp.panY.ToString(format);

        //fractalJulia.panY = Mathf.Min((float) fractalJulia.panY, 2);

        RefreshFractalJulia();
    }

    private void DownJulia(double defaultMovement){
        fractalJulia.rp.panY -= defaultMovement;

        textPanYJ.text = fractalJulia.rp.panY.ToString(format);


        //fractalJulia.panY = Mathf.Max((float) fractalJulia.panY, -2);

        RefreshFractalJulia();
    }

    private void LeftJulia(double defaultMovement){
        fractalJulia.rp.panX -= defaultMovement;

        textPanXJ.text = fractalJulia.rp.panX.ToString(format);

        //fractalJulia.panX = Mathf.Max((float) fractalJulia.panX, -2);

        RefreshFractalJulia();
    }

    private void RightJulia(double defaultMovement){
        fractalJulia.rp.panX += defaultMovement;

        textPanXJ.text = fractalJulia.rp.panX.ToString(format);


        //fractalJulia.panX = Mathf.Min((float) fractalJulia.panX, 2);

        RefreshFractalJulia();
    }


    public void RefreshFractalJulia(){
        UpdateJuliaRenderingValues();
        fractalJulia.RedrawCurrent();
        progressBarControllerJulia.StartProgressBarJulia();
    }

    private void RefreshFractalJulia(double rez, double imz){
        UpdateJuliaRenderingValues();
        fractalJulia.StartDraw(rez, imz);
        progressBarControllerJulia.StartProgressBarJulia();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && coordinatesListener.getIsPointerIn()){
            fractalJulia.StartDraw(rez, imz);
            realPartJulia.text = rez + "";
            imaginaryPartJulia.text = imz + "";
            progressBarControllerJulia.StartProgressBarJulia();
        }

        if (allowEnter && (Input.GetKey (KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))) {
            StartCoroutine(OnSubmit ());
            allowEnter = false;
        } else {
            foreach(TMP_InputField input in inputFields){
                allowEnter = allowEnter || input.isFocused;
            }
        }

        if (allowEnterJulia && (Input.GetKey (KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))){
            StartCoroutine(OnSubmitJulia());
            allowEnterJulia = false;
        }else{
            allowEnterJulia = realPartJulia.isFocused || imaginaryPartJulia.isFocused;
        }
        
    }

    private IEnumerator OnSubmitJulia(){
        yield return new WaitForEndOfFrame();
        double result;
        bool error = false;
        bool changed = false;
        for (int i = 0; i < 2; i++){
            if (!string.Equals(inputFieldsJulia[i].text, previousValuesJulia[i])){
                changed = true;
            }
            if (inputFieldsJulia[i].text.Length == 0){
                LogsController.UpdateLogs(new string[] {inputFieldsJulia[i].name + " is empty!"}, "#FFA600");
                yield return new WaitForSeconds(0.5f);
                inputFieldsJulia[i].text = previousValuesJulia[i];
                error = true;
            }
            if (double.TryParse(inputFieldsJulia[i].text, out result)){
                inputFieldsJulia[i].text = result.ToString(format);
                continue;
            }else{
                LogsController.UpdateLogs(new string[] {"Error parsing " + inputFieldsJulia[i].name + ". Cannot parse '" + inputFieldsJulia[i].text + "' to double."}, "#FFA600");
                yield return new WaitForSeconds(0.5f);
                inputFieldsJulia[i].text = previousValuesJulia[i];
                error = true;
            }
        }
        if (!error && changed){
            rez = double.Parse(inputFieldsJulia[0].text);
            imz = double.Parse(inputFieldsJulia[1].text);
            RefreshFractalJulia(rez, imz);
        }

        for (int i = 0; i < previousValuesJulia.Count - 1; i++){
            previousValuesJulia[i] = inputFieldsJulia[i].text;
        }
    }

    private IEnumerator OnSubmit(){
        yield return new WaitForEndOfFrame();
        double result;
        bool error = false;
        bool changed = false;
        for (int i = 0; i < 3; i++){
            if (!string.Equals(inputFields[i].text, previousValues[i])){
                changed = true;
            }
            if (inputFields[i].text.Length == 0){
                LogsController.UpdateLogs(new string[] {inputFields[i].name + " is empty!"}, "#FFA600");
                yield return new WaitForSeconds(0.5f);
                inputFields[i].text = previousValues[i];
                error = true;
            }
            if (double.TryParse(inputFields[i].text, out result)){
                inputFields[i].text = result.ToString(format);
                continue;
            }else{
                LogsController.UpdateLogs(new string[] {"Error parsing Mandelbrot " + inputFields[i].name + ". Cannot parse '" + inputFields[i].text + "' to double."}, "#FFA600");
                yield return new WaitForSeconds(0.5f);
                inputFields[i].text = previousValues[i];
                error = true;
            }
        }
        UpdateMandelbrotRenderingValues();
        if (!error && changed){
            RefreshFractalMandelbrot();
        }

        yield return new WaitForEndOfFrame();
        error = false;
        changed = false;
        for (int i = 3; i < 6; i++){
            if (!string.Equals(inputFields[i].text, previousValues[i])){
                changed = true;
            }
            if (double.TryParse(inputFields[i].text, out result)){
                inputFields[i].text = result.ToString(format);
                continue;
            }else{
                LogsController.UpdateLogs(new string[] {"Error parsing Julia " + inputFields[i].name + ". Cannot parse '"+inputFields[i].text+ "' to double."}, "#FFA600");
                yield return new WaitForSeconds(0.5f);
                inputFields[i].text = previousValues[i];
                error = true;
            }
        }
        UpdateJuliaRenderingValues();
        if (!error && changed){
            RefreshFractalJulia();
        }

        for (int i = 0; i < 6; i++){
            previousValues[i] = inputFields[i].text;
        } 

    }


    private void UpdateMandelbrotRenderingValues(){
        fractalMandelbrot.rp.xmax = Double.Parse(inputFields[0].text);
        fractalMandelbrot.rp.ymax = Double.Parse(inputFields[0].text);
        fractalMandelbrot.rp.panX = Double.Parse(inputFields[1].text);
        fractalMandelbrot.rp.panY = Double.Parse(inputFields[2].text);
    }

    private void UpdateJuliaRenderingValues(){
        fractalJulia.rp.xmax = Double.Parse(inputFields[3].text);
        fractalJulia.rp.ymax = Double.Parse(inputFields[3].text);
        fractalJulia.rp.panX = Double.Parse(inputFields[4].text);
        fractalJulia.rp.panY = Double.Parse(inputFields[5].text);
    }

    IEnumerator ListenerFractal(){
        while (true){
            if (coordinatesListener.getIsPointerIn()){
                rez = fractalMandelbrot.GetViewPortX(coordinatesListener.getX());
                imz = fractalMandelbrot.GetViewPortY(coordinatesListener.getY());
                if (imz < 0) mandelbrotNumber.text = "z = "+ rez + " - i "+ Math.Abs(imz);
                else mandelbrotNumber.text = "z = "+ rez + " + i "+ imz;
            }
            yield return new WaitForEndOfFrame();

        }
    }


}
