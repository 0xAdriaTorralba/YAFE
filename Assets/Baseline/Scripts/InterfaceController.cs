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

    private TextMeshProUGUI mandelbrotNumber, juliaNumber, numImagesJuliaText;

    private MandelbrotCPU fractalMandelbrot;
    private JuliaCPU fractalJulia;

    private CoordinatesListener clMandelbrot, clJulia;
    private GameObject eventSystem;

    private Button zoomInMandelbrot, zoomOutMandelbrot, refreshMandelbrot, upMandelbrot, downMandelbrot, leftMandelbrot, rightMandelbrot;

    private Button zoomInJulia, zoomOutJulia, refreshJulia, upJulia, downJulia, leftJulia, rightJulia;

    private TMP_InputField textZoomM, textPanXM, textPanYM;
    private TMP_InputField textZoomJ, textPanXJ, textPanYJ;

    private TMP_InputField realPartJulia, imaginaryPartJulia;

    private Toggle parallelToggle;

    private Slider sliderNumImagesJulia;

    private const string format = "F14";

    private double rezM, imzM, rezJ, imzJ;

    private List<TMP_InputField> inputFields, inputFieldsJulia;

    private List<string> previousValues, previousValuesJulia;
    
    private bool allowEnter, allowEnterJulia, calculateJuliaImages = false, reverse = false;
    private double defaultZoom, defaultMovement;

    void Awake()
    {
        
        // Finds
        mandelbrotNumber = GameObject.Find("Complex Number Mandelbrot/Mandelbrot Text").GetComponent<TextMeshProUGUI>();
        juliaNumber = GameObject.Find("Complex Number Julia/Julia Text").GetComponent<TextMeshProUGUI>();
        numImagesJuliaText = GameObject.Find("Num Images Label").GetComponent<TextMeshProUGUI>();


        fractalMandelbrot = GameObject.FindGameObjectWithTag("Mandelbrot").GetComponent<MandelbrotCPU>();
        fractalJulia = GameObject.FindGameObjectWithTag("Julia").GetComponent<JuliaCPU>();



        clMandelbrot = GameObject.FindGameObjectWithTag("Mandelbrot").GetComponent<CoordinatesListener>();
        clJulia = GameObject.FindGameObjectWithTag("Julia").GetComponent<CoordinatesListener>();


        textZoomM = GameObject.Find("Utilities Mandelbrot/Text Inputs/Zoom Input Field").GetComponent<TMP_InputField>();
        textPanXM = GameObject.Find("Utilities Mandelbrot/Text Inputs/Pan X Input Field").GetComponent<TMP_InputField>();
        textPanYM = GameObject.Find("Utilities Mandelbrot/Text Inputs/Pan Y Input Field").GetComponent<TMP_InputField>();

        textZoomJ = GameObject.Find("Utilities Julia/Text Inputs/Zoom Input Field").GetComponent<TMP_InputField>();
        textPanXJ = GameObject.Find("Utilities Julia/Text Inputs/Pan X Input Field").GetComponent<TMP_InputField>();
        textPanYJ = GameObject.Find("Utilities Julia/Text Inputs/Pan Y Input Field").GetComponent<TMP_InputField>();

        realPartJulia = GameObject.Find("Complex Number Julia/Input Real Julia").GetComponent<TMP_InputField>();
        imaginaryPartJulia = GameObject.Find("Complex Number Julia/Input Imaginary Julia").GetComponent<TMP_InputField>();

        parallelToggle = GameObject.FindGameObjectWithTag("ParallelToggle").GetComponent<Toggle>();
        
        //WebGL does not support CPU Threads at moment, so we need to deactivate this feature :(
        #if UNITY_WEBGL && !UNITY_EDITOR
            fractalJulia.rp.parallel = false;
            fractalMandelbrot.rp.parallel = false;
            parallelToggle.isOn = false;
            parallelToggle.interactable = false;
        #endif

        sliderNumImagesJulia = GameObject.Find("Slider Num Images Julia").GetComponent<Slider>();

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

        parallelToggle.onValueChanged.AddListener((value) => ToggleParallel(value));

        sliderNumImagesJulia.onValueChanged.AddListener((value) => OnValueChangedSlider(sliderNumImagesJulia));
        
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
        StartCoroutine(ListenerFractal(fractalMandelbrot, clMandelbrot, mandelbrotNumber));
        StartCoroutine(ListenerFractal(fractalJulia, clJulia, juliaNumber));
        StartCoroutine(DrawImagesJulia());
    }

    private IEnumerator DrawImagesJulia(){
        int x = (int) clJulia.getX();
        int y = (int) clJulia.getY();
        bool valuesChanged = true;
        bool imageCleaned = true;
        while(true){
            if ((Input.GetKey(KeyCode.P) || Input.GetMouseButton(0)) && clJulia.getIsPointerIn() && fractalJulia.GetFinished()){
                if (valuesChanged){
                    fractalJulia.RestoreImage();
                    x = (int) clJulia.getX();
                    y = (int) clJulia.getY();
                    fractalJulia.CalculateImageAndDrawImage(rezJ, imzJ, x, y);
                    imageCleaned = false;
                    valuesChanged = false;
                }else{
                    if (x != (int) clJulia.getX() || y != (int) clJulia.getY()){
                        valuesChanged = true;
                    }else{
                        valuesChanged = false;
                    }
                }
            }else{
                if (!imageCleaned){
                    fractalJulia.RestoreImage();
                    imageCleaned = true;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }



    private void ToggleParallel(bool value){
        parallelToggle.isOn = value;
        fractalMandelbrot.rp.parallel = parallelToggle.isOn;
        fractalJulia.rp.parallel = parallelToggle.isOn;
        RefreshFractalMandelbrot();
        RefreshFractalJulia();
    }

    private void OnValueChangedSlider(Slider slider){
        fractalJulia.SetNumImages((int) slider.value);
        numImagesJuliaText.text = (int) slider.value + "";
    }

    private void ZoomInMandelbrot(double defaultZoom){
        fractalMandelbrot.rp.xmax *= defaultZoom;
        fractalMandelbrot.rp.ymax *= defaultZoom;

        Clamp(ref fractalMandelbrot.rp.xmax, 4e-13, 2.0);
        Clamp(ref fractalMandelbrot.rp.ymax, 4e-13, 2.0);


        textZoomM.text = fractalMandelbrot.rp.xmax.ToString(format);

        //fractalMandelbrot.xmax = Mathf.Max((float)fractalMandelbrot.xmax, 1e-14f);
        //fractalMandelbrot.ymax = Mathf.Max((float)fractalMandelbrot.ymax, 1e-14f);

        RefreshFractalMandelbrot();
    }

    private void ZoomOutMandelbrot(double defaultZoom){
        fractalMandelbrot.rp.xmax /= defaultZoom;
        fractalMandelbrot.rp.ymax /= defaultZoom;
        
        Clamp(ref fractalMandelbrot.rp.xmax, 4e-13, 2.0);
        Clamp(ref fractalMandelbrot.rp.ymax, 4e-13, 2.0);

        textZoomM.text = fractalMandelbrot.rp.xmax.ToString(format);

        //fractalMandelbrot.xmax = Mathf.Min((float)fractalMandelbrot.xmax, 2);
        //fractalMandelbrot.ymax = Mathf.Min((float)fractalMandelbrot.ymax, 2);

        RefreshFractalMandelbrot();
    }

    private void UpMandelbrot(double defaultMovement){
        fractalMandelbrot.rp.panY += defaultMovement;

        Clamp(ref fractalMandelbrot.rp.panY, -2.0, 2.0);

        textPanYM.text = fractalMandelbrot.rp.panY.ToString(format);

        //fractalMandelbrot.panY = Mathf.Min((float) fractalMandelbrot.panY, 2);

        RefreshFractalMandelbrot();
    }

    private void DownMandelbrot(double defaultMovement){
        fractalMandelbrot.rp.panY -= defaultMovement;

        Clamp(ref fractalMandelbrot.rp.panY, -2.0, 2.0);

        textPanYM.text = fractalMandelbrot.rp.panY.ToString(format);

        //fractalMandelbrot.panY = Mathf.Max((float) fractalMandelbrot.panY, -2);

        RefreshFractalMandelbrot();
    }

    private void LeftMandelbrot(double defaultMovement){
        fractalMandelbrot.rp.panX -= defaultMovement;

        Clamp(ref fractalMandelbrot.rp.panX, -2.0, 2.0);


        textPanXM.text = fractalMandelbrot.rp.panX.ToString(format);

        //fractalMandelbrot.panX = Mathf.Max((float) fractalMandelbrot.panX, -2);

        RefreshFractalMandelbrot();
    }

    private void RightMandelbrot(double defaultMovement){
        fractalMandelbrot.rp.panX += defaultMovement;
    
        Clamp(ref fractalMandelbrot.rp.panY, -2.0, 2.0);

        textPanXM.text = fractalMandelbrot.rp.panX.ToString(format);


        //fractalMandelbrot.panX = Mathf.Min((float) fractalMandelbrot.panX, 2);

        RefreshFractalMandelbrot();
    }

    public void RefreshFractalMandelbrot(){
        UpdateMandelbrotRenderingValues();
        fractalMandelbrot.StartDraw();
        //progressBarControllerMandelbrot.StartProgressBarMandelbrot();
    }

    private void ZoomInJulia(double defaultZoom){
        fractalJulia.rp.xmax *= defaultZoom;
        fractalJulia.rp.ymax *= defaultZoom;

        Clamp(ref fractalJulia.rp.xmax, 4e-13, 2.0);
        Clamp(ref fractalJulia.rp.ymax, 4e-13, 2.0);

        textZoomJ.text = fractalJulia.rp.xmax.ToString(format);

        //fractalJulia.xmax = Mathf.Max((float)fractalJulia.xmax, 1e-14f);
        //fractalJulia.ymax = Mathf.Max((float)fractalJulia.ymax, 1e-14f);

        RefreshFractalJulia();
    }

    private void ZoomOutJulia(double defaultZoom){
        fractalJulia.rp.xmax /= defaultZoom;
        fractalJulia.rp.ymax /= defaultZoom;

        Clamp(ref fractalJulia.rp.xmax, 4e-13, 2.0);
        Clamp(ref fractalJulia.rp.ymax, 4e-13, 2.0);

        textZoomJ.text = fractalJulia.rp.xmax.ToString(format);


        //fractalJulia.xmax = Mathf.Min((float)fractalJulia.xmax, 2);
        //fractalJulia.ymax = Mathf.Min((float)fractalJulia.ymax, 2);

        RefreshFractalJulia();
    }

       private void UpJulia(double defaultMovement){
        fractalJulia.rp.panY += defaultMovement;

        Clamp(ref fractalJulia.rp.panY, -2.0, 2.0);

        textPanYJ.text = fractalJulia.rp.panY.ToString(format);

        //fractalJulia.panY = Mathf.Min((float) fractalJulia.panY, 2);

        RefreshFractalJulia();
    }

    private void DownJulia(double defaultMovement){
        fractalJulia.rp.panY -= defaultMovement;

        Clamp(ref fractalJulia.rp.panY, -2.0, 2.0);


        textPanYJ.text = fractalJulia.rp.panY.ToString(format);


        //fractalJulia.panY = Mathf.Max((float) fractalJulia.panY, -2);

        RefreshFractalJulia();
    }

    private void LeftJulia(double defaultMovement){
        fractalJulia.rp.panX -= defaultMovement;

        Clamp(ref fractalJulia.rp.panX, -2.0, 2.0);

        textPanXJ.text = fractalJulia.rp.panX.ToString(format);

        //fractalJulia.panX = Mathf.Max((float) fractalJulia.panX, -2);

        RefreshFractalJulia();
    }

    private void RightJulia(double defaultMovement){
        fractalJulia.rp.panX += defaultMovement;

        Clamp(ref fractalJulia.rp.panX, -2.0, 2.0);

        textPanXJ.text = fractalJulia.rp.panX.ToString(format);


        //fractalJulia.panX = Mathf.Min((float) fractalJulia.panX, 2);

        RefreshFractalJulia();
    }


    public void RefreshFractalJulia(){
        UpdateJuliaRenderingValues();
        fractalJulia.RedrawCurrent();
        //progressBarControllerJulia.StartProgressBarJulia();
    }

    private void RefreshFractalJulia(double rez, double imz){
        UpdateJuliaRenderingValues();
        fractalJulia.StartDraw(rez, imz);
        //progressBarControllerJulia.StartProgressBarJulia();
    }

    private void ZoomInFractal(Fractal fractal, double rez, double imz){
        fractal.rp.panX = rez;
        fractal.rp.panY = imz;
        fractal.rp.xmax *= defaultZoom;
        fractal.rp.ymax *= defaultZoom;
        if (fractal is MandelbrotCPU){
            inputFields[0].text = fractal.rp.xmax.ToString(format);
            inputFields[1].text = fractal.rp.panX.ToString(format);
            inputFields[2].text = fractal.rp.panY.ToString(format);
            ((MandelbrotCPU) fractal).StartDraw();
        }
        if (fractal is JuliaCPU){
            inputFields[3].text = fractal.rp.xmax.ToString(format);
            inputFields[4].text = fractal.rp.panX.ToString(format);
            inputFields[5].text = fractal.rp.panY.ToString(format);
            ((JuliaCPU) fractal).RedrawCurrent();
        }
    }

    private void ZoomOutFractal(Fractal fractal, double rez, double imz){
        fractal.rp.panX = rez;
        fractal.rp.panY = imz;
        fractal.rp.xmax /= defaultZoom;
        fractal.rp.ymax /= defaultZoom;
        if (fractal is MandelbrotCPU){
            inputFields[0].text = fractal.rp.xmax.ToString(format);
            inputFields[1].text = fractal.rp.panX.ToString(format);
            inputFields[2].text = fractal.rp.panY.ToString(format);
            ((MandelbrotCPU) fractal).StartDraw();
        }
        if (fractal is JuliaCPU){
            inputFields[3].text = fractal.rp.xmax.ToString(format);
            inputFields[4].text = fractal.rp.panX.ToString(format);
            inputFields[5].text = fractal.rp.panY.ToString(format);
            ((JuliaCPU) fractal).RedrawCurrent();
        }
    }

    private void ResetInputFieldsJulia(){
        inputFields[3].text = "" + 2.0.ToString(format);
        inputFields[4].text = "" + 0.0.ToString(format);
        inputFields[5].text = "" + 0.0.ToString(format);
        previousValues[3] = "" + 2.0.ToString(format);
        previousValues[4] = "" + 0.0.ToString(format);
        previousValues[5] = "" + 0.0.ToString(format);
    }


    // Update is called once per frame
    void Update()
    {
        // Choose 'c' value for Julia
        if(Input.GetMouseButtonDown(0) && clMandelbrot.getIsPointerIn()){
            fractalJulia.ResetRenderParameters();
            ResetInputFieldsJulia();
            fractalJulia.StartDraw(rezM, imzM);
            realPartJulia.text = rezM + "";
            imaginaryPartJulia.text = imzM + "";
        }

        // Zoom over Mandelbrot.
        if(Input.GetMouseButtonDown(1) && clMandelbrot.getIsPointerIn()){
            if (Input.GetKey(KeyCode.LeftShift)){
                ZoomOutFractal(fractalMandelbrot, rezM, imzM);
            }else{
                ZoomInFractal(fractalMandelbrot, rezM, imzM);
            }
        }

        // Zoom over Julia.
        if(Input.GetMouseButtonDown(1) && clJulia.getIsPointerIn()){
            if (Input.GetKey(KeyCode.LeftShift)){
                ZoomOutFractal(fractalJulia, rezJ, imzJ);
            }else{
                ZoomInFractal(fractalJulia, rezJ, imzJ);
            }

        }

        // Rendering Input Fields
        if (allowEnter && (Input.GetKey (KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))) {
            StartCoroutine(OnSubmit ());
            allowEnter = false;
        } else {
            foreach(TMP_InputField input in inputFields){
                allowEnter = allowEnter || input.isFocused;
            }
        }

        // Julia c Input Fields
        if (allowEnterJulia && (Input.GetKey (KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))){
            StartCoroutine(OnSubmitJulia());
            allowEnterJulia = false;
        }else{
            allowEnterJulia = realPartJulia.isFocused || imaginaryPartJulia.isFocused;
        }

        if (Input.GetKey(KeyCode.LeftShift)){
            reverse = true;
        }else{
            reverse = false;
        }

        // Input Fields management through Tab and LShifh + Tabs :)
        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (reverse){
                if (allowEnter){
                    for (int i = 5; i >= 0; i--){
                        if(inputFields[i].isFocused){
                            if (i != 0){
                                inputFields[i].text = previousValues[i];
                                inputFields[(i-1) % 6].Select();
                            }else{
                                inputFields[0].text = previousValues[0];
                                inputFields[5].Select();
                            }
                            break;
                        }
                    }
                }
                if (allowEnterJulia){
                    for (int i = 1; i >= 0; i--){
                        if (inputFieldsJulia[i].isFocused){
                            inputFieldsJulia[i].text = previousValuesJulia[i];
                            inputFieldsJulia[(i-1) % 2].Select();
                        }else{
                            inputFieldsJulia[0].text = previousValuesJulia[0];
                            inputFieldsJulia[1].Select();
                        }
                        break;
                    }
                }
            }else{
                if (allowEnter){
                    for (int i = 0; i < 6; i++){
                        if(inputFields[i].isFocused){
                            inputFields[(i+1) % 6].Select();
                            break;
                        }
                    }
                }
                if (allowEnterJulia){
                    for (int i = 0; i < 2; i++){
                        if (inputFieldsJulia[i].isFocused){
                            inputFieldsJulia[(i+1) % 2].Select();
                            break;
                        }
                    }
                }
            }

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
            rezM = double.Parse(inputFieldsJulia[0].text);
            imzM = double.Parse(inputFieldsJulia[1].text);
            RefreshFractalJulia(rezM, imzM);
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
                switch (inputFields[i].name)
                {
                    case "Zoom Input Field":
                        Clamp(ref result, 4e-13, 2.0, "Mandelbrot "+inputFields[i].name);
                        break;
                    case "Pan X Input Field":
                        Clamp(ref result, -2.0, 2.0, "Mandelbrot "+inputFields[i].name);
                        break;
                    case "Pan Y Input Field":
                        Clamp(ref result, -2.0, 2.0, "Mandelbrot "+inputFields[i].name);
                        break;
                    default:
                        break;
                }
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
                switch (inputFields[i].name)
                {
                    case "Zoom Input Field":
                        Clamp(ref result, 4e-13, 2.0, "Julia "+inputFields[i].name);
                        break;
                    case "Pan X Input Field":
                         Clamp(ref result, -2.0, 2.0, "Julia "+inputFields[i].name);
                         break;
                    case "Pan Y Input Field":
                        Clamp(ref result, -2.0, 2.0, "Julia "+inputFields[i].name);
                        break;
                    default:
                        break;
                }
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

    IEnumerator ListenerFractal(Fractal fractal, CoordinatesListener coordinatesListener, TextMeshProUGUI text){
        while (true){
            if (coordinatesListener.getIsPointerIn()){
                
                if (fractal is MandelbrotCPU){
                    rezM = fractal.GetViewPortX(coordinatesListener.getX());
                    imzM = fractal.GetViewPortY(coordinatesListener.getY());
                    if (imzM < 0) text.text = "c = "+ rezM.ToString(format) + " - i "+ Math.Abs(imzM).ToString(format);
                    else text.text = "c = "+ rezM.ToString(format) + " + i "+ imzM.ToString(format);
                }else if (fractal is JuliaCPU){
                    rezJ = fractal.GetViewPortX(coordinatesListener.getX());
                    imzJ = fractal.GetViewPortY(coordinatesListener.getY());
                    if (imzJ < 0) text.text = "z = "+ rezJ.ToString(format) + " - i "+ Math.Abs(imzJ).ToString(format);
                    else text.text = "z = "+ rezJ.ToString(format) + " + i "+ imzJ.ToString(format);
                }
                
            }
            yield return new WaitForEndOfFrame();

        }
    }

    private void Clamp(ref double value, double min, double max){
        if (value < min){
            value = min;
            //LogsController.UpdateLogs(new string[] {"You have reached the minimum value for this parameter."}, "#FFA600");
        }
        if (value > max){
            value = max;
            //LogsController.UpdateLogs(new string[] {"You have reached the maximum value for this parameter."}, "#FFA600");

        }
    }

    private void Clamp(ref double value, double min, double max, string source){
        if (value < min){
            value = min;
            LogsController.UpdateLogs(new string[] {"You have reached the minimum value for " + source + "."}, "#FFA600");
        }
        if (value > max){
            value = max;
            LogsController.UpdateLogs(new string[] {"You have reached the maximum value for " + source + "."}, "#FFA600");
        }
    }


}
