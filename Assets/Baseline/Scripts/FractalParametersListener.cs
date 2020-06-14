using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FractalParametersListener : MonoBehaviour
{

    private Fractal fractalMandelbrot;

    private GameObject detail, threshold;
    private Fractal fractalJulia;

    private InterfaceController interfaceController;

    private TMP_Dropdown dropdownAlgorithm, dropdownColormap, dropdownFamily;
    private Slider sliderMaxIters, sliderThreshold, sliderDegree, sliderDetail;
    
    private int previousMaxIters = 100, previousThreshold = 2, previousDegree = 2, previousDetail = 3000;

    private TMP_InputField inputFieldMaxIters, inputFieldThreshold, inputFieldDegree, inputFieldDetail;

    private bool allowEnterMaxIters = false, allowEnterThreshold = false, allowEnterDegree = false, allowEnterDetail = false;
    

    void Awake(){
        threshold = GameObject.Find("Threshold");
        detail = GameObject.Find("Detail");
        dropdownAlgorithm = GameObject.FindGameObjectWithTag("DropdownAlgorithm").GetComponent<TMP_Dropdown>();
        dropdownColormap = GameObject.FindGameObjectWithTag("DropdownColormap").GetComponent<TMP_Dropdown>();
        dropdownFamily = GameObject.FindGameObjectWithTag("DropdownFamily").GetComponent<TMP_Dropdown>();
        
        sliderMaxIters = GameObject.FindGameObjectWithTag("SliderMaxIters").GetComponent<Slider>();
        sliderThreshold = GameObject.FindGameObjectWithTag("SliderThreshold").GetComponent<Slider>();
        sliderDegree = GameObject.FindGameObjectWithTag("SliderDegree").GetComponent<Slider>();
        sliderDetail = GameObject.FindGameObjectWithTag("SliderDetail").GetComponent<Slider>();

        inputFieldMaxIters = GameObject.FindGameObjectWithTag("InputFieldMaxIters").GetComponent<TMP_InputField>();
        inputFieldThreshold = GameObject.FindGameObjectWithTag("InputFieldThreshold").GetComponent<TMP_InputField>();
        inputFieldDegree = GameObject.FindGameObjectWithTag("InputFieldDegree").GetComponent<TMP_InputField>();
        inputFieldDetail = GameObject.FindGameObjectWithTag("InputFieldDetail").GetComponent<TMP_InputField>();

        fractalMandelbrot = GameObject.FindGameObjectWithTag("Mandelbrot").GetComponent<Fractal>();
        fractalJulia = GameObject.FindGameObjectWithTag("Julia").GetComponent<Fractal>();

        interfaceController = GameObject.FindGameObjectWithTag("InterfaceController").GetComponent<InterfaceController>();

    }

    void Start()
    {
       
        dropdownAlgorithm.onValueChanged.AddListener( delegate { DropdownValueChanged(dropdownAlgorithm); });
        dropdownColormap.onValueChanged.AddListener( delegate { DropdownValueChanged(dropdownColormap); });
        dropdownFamily.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdownFamily); });

        sliderMaxIters.onValueChanged.AddListener( delegate { SliderValueChanged(sliderMaxIters); });
        sliderThreshold.onValueChanged.AddListener( delegate { SliderValueChanged(sliderThreshold); });
        sliderDegree.onValueChanged.AddListener( delegate { SliderValueChanged(sliderDegree); });
        sliderDetail.onValueChanged.AddListener( delegate { SliderValueChanged(sliderDetail); });
        //inputFieldMaxIters.onValueChanged.AddListener( delegate  { InputFieldValueChanged(inputFieldMaxIters); });
        ToggleParameters(true, false);
    }

    public void OnSliderEnd(){
        interfaceController.RefreshFractalMandelbrot();
        interfaceController.RefreshFractalJulia();
    }

    private void DropdownValueChanged(TMP_Dropdown dropdown){
        if (string.Equals(dropdown.tag, "DropdownAlgorithm")){
            fractalMandelbrot.fp.algorithm = dropdown.captionText.text.ToString();
            fractalJulia.fp.algorithm = dropdown.captionText.text.ToString();
            switch(dropdown.captionText.text.ToString()){
                case "Escape Algorithm":
                    ToggleParameters(true, false);
                    break;
                case "Henriksen Algorithm":
                    ToggleParameters(false, true);
                    break;
                default:
                    ToggleParameters(true, false);
                    break;
            }
            interfaceController.RefreshFractalMandelbrot();
            interfaceController.RefreshFractalJulia();
        }
        if (string.Equals(dropdown.tag, "DropdownColormap")){
            fractalMandelbrot.fp.colorMap = dropdown.captionText.text.ToString();
            fractalJulia.fp.colorMap = dropdown.captionText.text.ToString();
            interfaceController.RefreshFractalMandelbrot();
            interfaceController.RefreshFractalJulia();
        }
        if (string.Equals(dropdown.tag, "DropdownFamily")){
            fractalMandelbrot.fp.family = dropdown.captionText.text.ToString();
            fractalJulia.fp.family = dropdown.captionText.text.ToString();
            interfaceController.RefreshFractalMandelbrot();
            interfaceController.RefreshFractalJulia();
        }
    }

    private void SliderValueChanged(Slider slider){
        if (string.Equals(slider.tag, "SliderMaxIters")){
            // if ((int) slider.value < 200){
            //     fractalMandelbrot.fp.maxIters = 200;
            //     fractalJulia.fp.maxIters = 200;
            //     inputFieldMaxIters.text = 200+"";
            // } 
            // if ((int) slider.value > 200 && (int) slider.value < 500){
            //     fractalMandelbrot.fp.maxIters = 500;
            //     fractalJulia.fp.maxIters = 500;
            //     inputFieldMaxIters.text = 500+"";
            // }
            // if ((int) slider.value > 500){
            //     fractalMandelbrot.fp.maxIters = 800;
            //     fractalJulia.fp.maxIters = 800;
            //     inputFieldMaxIters.text = 800+"";
            // } 
            fractalMandelbrot.fp.maxIters = (int) slider.value;
            fractalJulia.fp.maxIters = (int) slider.value;
            inputFieldMaxIters.text = slider.value + "";
        }
        if (string.Equals(slider.tag, "SliderThreshold")){
            fractalMandelbrot.fp.threshold = (int) slider.value;
            fractalJulia.fp.threshold = (int) slider.value;
            inputFieldThreshold.text = slider.value + "";
        }
        if (string.Equals(slider.tag, "SliderDegree")){
            fractalMandelbrot.fp.degree = (int) slider.value;
            fractalJulia.fp.degree = (int) slider.value;
            inputFieldDegree.text = slider.value + "";
        }
        if (string.Equals(slider.tag, "SliderDetail")){
            fractalMandelbrot.fp.detail = (int) slider.value;
            fractalJulia.fp.detail = (int) slider.value;
            inputFieldDetail.text = slider.value + "";
        }
    }

    void Update(){


        // Capture MaxIters input field.
        if (allowEnterMaxIters && (Input.GetKey (KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))){
            OnSubmit(inputFieldMaxIters, ref previousMaxIters, 10, 1000);
            allowEnterMaxIters = false;
        }else{
            allowEnterMaxIters = inputFieldMaxIters.isFocused;
        }

        // Capture Threshold input field.
        if (allowEnterThreshold && (Input.GetKey (KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))){
            OnSubmit(inputFieldThreshold, ref previousThreshold, 2, 100);
            allowEnterThreshold = false;
        }else{
            allowEnterThreshold = inputFieldThreshold.isFocused;
        }

        // Capture Detail input field.
        if (allowEnterDetail && (Input.GetKey (KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))){
            OnSubmit(inputFieldDetail, ref previousDetail, 3000, 10000);
            allowEnterDetail = false;
        }else{
            allowEnterDetail = inputFieldDetail.isFocused;
        }

        // Capture Degree input field.
        if (allowEnterDegree && (Input.GetKey (KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))){
            OnSubmit(inputFieldDegree, ref previousDegree, 2, 8);
            allowEnterDegree = false;
        }else{
            allowEnterDegree = inputFieldDegree.isFocused;
        }


    }

     private void OnSubmit(TMP_InputField inputField, ref int previousValue, int min, int max){
        int result;
        bool changed = false;
        bool error = false;
        if (!string.Equals(inputField.text, previousValue + "")){
            changed = true;
        }
        if (inputField.text.Length == 0){
            LogsController.UpdateLogs(new string[] {inputField.name + " is empty!"}, "#FFA600");
            inputField.text = previousValue + "";
            error = true;
        }
        if (int.TryParse(inputField.text, out result)){
            if (result >= min && result <= max){
                if (string.Equals(inputField.tag, "InputFieldMaxIters")){
                    fractalMandelbrot.fp.maxIters = fractalJulia.fp.maxIters = result;
                    sliderMaxIters.value = result;
                    previousValue = result;
                }
                if (string.Equals(inputField.tag, "InputFieldThreshold")){
                    fractalMandelbrot.fp.threshold = fractalJulia.fp.threshold = result;
                    sliderThreshold.value = result;
                    previousValue = result;
                }
                if (string.Equals(inputField.tag, "InputFieldDegree")){
                    fractalMandelbrot.fp.degree = fractalJulia.fp.degree = result;
                    sliderDegree.value = result;
                    previousValue = result;
                }
                if (string.Equals(inputField.tag, "InputFieldDetail")){
                    fractalMandelbrot.fp.detail = fractalJulia.fp.detail = result;
                    sliderDetail.value = result;
                    previousValue = result;
                }
                error = false;
            }else{
                LogsController.UpdateLogs(new string[] {"You must input a value between " + min + " and " + max +"."}, "#FFA600");
                inputField.text = previousValue.ToString("D");
                error = true;
            }
        }else{
            LogsController.UpdateLogs(new string[] {"Error parsing " + inputField.name + ". Cannot parse '" + inputField.text + "' to int."}, "#FFA600");
            inputField.text = previousValue.ToString("D");
            error = true;
        }

        if (!error && changed){
            interfaceController.RefreshFractalMandelbrot();
            interfaceController.RefreshFractalJulia();
        }
    }


    private void ToggleParameters(bool thr, bool det){
        threshold.SetActive(thr);
        detail.SetActive(det);
    }

}
