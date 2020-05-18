﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using TMPro;

public class InterfaceControllerGPU : MonoBehaviour
{
    // Start is called before the first frame update

    private TextMeshProUGUI mandelbrotNumber, juliaNumber;

    private MandelbrotGPU fractalMandelbrot;
    private JuliaGPU fractalJulia;

    private CoordinatesListener clMandelbrot, clJulia;
    private GameObject eventSystem;

    private CameraMovementController cameraMandelbrot, cameraJulia;

    private TMP_InputField textZoomM, textPanXM, textPanYM;
    private TMP_InputField textZoomJ, textPanXJ, textPanYJ;

    private TMP_InputField realPartJulia, imaginaryPartJulia;
    
    private Toggle smoothToggle;

    private Button refreshMandelbrot, refreshJulia;

    private const string format = "F7";

    private double rezM, imzM, rezJ, imzJ;

    private List<TMP_InputField> inputFields, inputFieldsJulia;

    private List<string> previousValues, previousValuesJulia;
    
    private bool allowEnter, allowEnterJulia;


    private double defaultZoom, defaultMovement;
    void Awake()
    {
        
        // Finds
        mandelbrotNumber = GameObject.Find("Complex Number Mandelbrot/Mandelbrot Text").GetComponent<TextMeshProUGUI>();
        juliaNumber = GameObject.Find("Complex Number Julia/Julia Text").GetComponent<TextMeshProUGUI>();

        fractalMandelbrot = GameObject.FindGameObjectWithTag("Mandelbrot").GetComponent<MandelbrotGPU>();
        fractalJulia = GameObject.FindGameObjectWithTag("Julia").GetComponent<JuliaGPU>();

        cameraMandelbrot = GameObject.FindGameObjectWithTag("Mandelbrot").GetComponent<CameraMovementController>();
        cameraJulia = GameObject.FindGameObjectWithTag("Julia").GetComponent<CameraMovementController>();


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

        smoothToggle = GameObject.FindGameObjectWithTag("SmoothToggle").GetComponent<Toggle>();

        refreshMandelbrot = GameObject.Find("Utilities Mandelbrot/Refresh").GetComponent<Button>();
        refreshJulia = GameObject.Find("Utilities Julia/Refresh").GetComponent<Button>();


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

        defaultZoom = 0.5;
        defaultMovement = 0.1;
        

    }

    void Start(){
 
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

        refreshMandelbrot.onClick.AddListener(() => RestartFractalMandelbrot());
        refreshJulia.onClick.AddListener(() => RestartFractalJulia());
        smoothToggle.onValueChanged.AddListener((value) => OnValueChangedToggle(value));
        
        allowEnter = false;
        allowEnterJulia = false;

        

    }

    void OnEnable(){
        StartCoroutine(ListenerFractal(fractalMandelbrot, clMandelbrot, mandelbrotNumber));
        StartCoroutine(ListenerFractal(fractalJulia, clJulia, juliaNumber));
    }

    private void OnValueChangedToggle(bool value){
        cameraMandelbrot.ToggleSmoothMovement();
        cameraJulia.ToggleSmoothMovement();
    }


    public void RestartFractalMandelbrot(){
        cameraMandelbrot.SetZoom(2.0f);
        cameraMandelbrot.SetPosition(0.0f, 0.0f);

    }

   

    public void RestartFractalJulia(){
        cameraJulia.SetZoom(2.0f);
        cameraJulia.SetPosition(0.0f, 0.0f);
    }

    private void RefreshFractalJulia(double rez, double imz){
        UpdateJuliaRenderingValues();
        fractalJulia.UpdateSeed((float)rez, (float) imz);
    }


    // Update is called once per frame
    void Update()
    {
        if (clMandelbrot.getIsPointerIn() && Input.GetKey(KeyCode.R)){
            RestartFractalMandelbrot();
        }

        if (clJulia.getIsPointerIn() && Input.GetKey(KeyCode.R)){
            RestartFractalJulia();
        }

        if(Input.GetMouseButtonDown(0) && clMandelbrot.getIsPointerIn()){
            fractalJulia.UpdateSeed((float)rezM, (float)imzM);
            realPartJulia.text = rezM + "";
            imaginaryPartJulia.text = imzM + "";
        }


        // if(Input.GetMouseButtonDown(0) && clJulia.getIsPointerIn()){
        //     fractalJulia.CalculateImageAndDrawImage(rezJ, imzJ, (int)clJulia.getX(), (int)clJulia.getY());
        // }

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

        if (!textPanXM.isFocused){
            textPanXM.text = fractalMandelbrot.rp.panX.ToString(format);
        }
        if (!textPanYM.isFocused){
            textPanYM.text = fractalMandelbrot.rp.panY.ToString(format);
        }
        if (!textZoomM.isFocused){
            textZoomM.text = fractalMandelbrot.rp.xmax.ToString(format);
        }

        if (!textPanXJ.isFocused){
            textPanXJ.text = fractalJulia.rp.panX.ToString(format);
        }
        if (!textPanYJ.isFocused){
            textPanYJ.text = fractalJulia.rp.panY.ToString(format);
        }
        if (!textZoomJ.isFocused){
            textZoomJ.text = fractalJulia.rp.xmax.ToString(format);
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
                inputFields[i].text = result.ToString(format);
                continue;
            }else{
                LogsController.UpdateLogs(new string[] {"Error parsing Mandelbrot " + inputFields[i].name + ". Cannot parse '" + inputFields[i].text + "' to double."}, "#FFA600");
                yield return new WaitForSeconds(0.5f);
                inputFields[i].text = previousValues[i];
                error = true;
            }
        }
        if (!error && changed){
            UpdateMandelbrotRenderingValues();
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
        if (!error && changed){
            UpdateJuliaRenderingValues();
        }

        for (int i = 0; i < 6; i++){
            previousValues[i] = inputFields[i].text;
        } 

    }


    private void UpdateMandelbrotRenderingValues(){
        fractalMandelbrot.UpdateZoom((float)Double.Parse(inputFields[0].text));
        cameraMandelbrot.SetZoom((float)Double.Parse(inputFields[0].text));
        fractalMandelbrot.UpdatePosition((float)Double.Parse(inputFields[1].text), (float)Double.Parse(inputFields[2].text));
        cameraMandelbrot.SetPosition((float)Double.Parse(inputFields[1].text), (float)Double.Parse(inputFields[2].text));

    }

    private void UpdateJuliaRenderingValues(){
        fractalJulia.UpdateZoom((float)Double.Parse(inputFields[3].text));
        cameraJulia.SetZoom((float)Double.Parse(inputFields[3].text));
        fractalJulia.UpdatePosition((float)Double.Parse(inputFields[4].text), (float)Double.Parse(inputFields[5].text));
        cameraJulia.SetPosition((float)Double.Parse(inputFields[4].text), (float)Double.Parse(inputFields[5].text));
    }

    IEnumerator ListenerFractal(FractalGPU fractal, CoordinatesListener coordinatesListener, TextMeshProUGUI text){
        while (true){
            if (coordinatesListener.getIsPointerIn()){
                if (fractal is MandelbrotGPU){
                    rezM = fractal.GetViewPortX(coordinatesListener.getX());
                    imzM = fractal.GetViewPortY(coordinatesListener.getY());
                    if (imzM < 0) text.text = "c = "+ rezM.ToString(format) + " - i "+ Math.Abs(imzM).ToString(format);
                    else text.text = "c = "+ rezM.ToString(format) + " + i "+ imzM.ToString(format);
                }else if (fractal is JuliaGPU){
                    rezJ = fractal.GetViewPortX(coordinatesListener.getX());
                    imzJ = fractal.GetViewPortY(coordinatesListener.getY());
                    if (imzJ < 0) text.text = "z = "+ rezJ.ToString(format) + " - i "+ Math.Abs(imzJ).ToString(format);
                    else text.text = "z = "+ rezJ.ToString(format) + " + i "+ imzJ.ToString(format);
                }
                
            }
            yield return new WaitForEndOfFrame();

        }
    }


}
