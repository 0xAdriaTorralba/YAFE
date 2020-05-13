using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FractalParametersListener : MonoBehaviour
{

    private Mandelbrot fractalMandelbrot;
    private Julia fractalJulia;

    private InterfaceController interfaceController;

    private TMP_Dropdown dropdownAlgorithm, dropdownColormap, dropdownFamily;
    private Slider sliderMaxIters, sliderThreshold, sliderDegree;
    
    private int previousMaxIters = 100, previousThreshold = 2, previousDegree = 2;

    private TMP_InputField inputFieldMaxIters, inputFieldThreshold, inputFieldDegree;

    private bool allowEnterMaxIters = false, allowEnterThreshold = false, allowEnterDegree = false;
    
    // Start is called before the first frame update
    void Start()
    {
        dropdownAlgorithm = GameObject.FindGameObjectWithTag("DropdownAlgorithm").GetComponent<TMP_Dropdown>();
        dropdownColormap = GameObject.FindGameObjectWithTag("DropdownColormap").GetComponent<TMP_Dropdown>();
        dropdownFamily = GameObject.FindGameObjectWithTag("DropdownFamily").GetComponent<TMP_Dropdown>();
        
        sliderMaxIters = GameObject.FindGameObjectWithTag("SliderMaxIters").GetComponent<Slider>();
        sliderThreshold = GameObject.FindGameObjectWithTag("SliderThreshold").GetComponent<Slider>();
        sliderDegree = GameObject.FindGameObjectWithTag("SliderDegree").GetComponent<Slider>();

        inputFieldMaxIters = GameObject.FindGameObjectWithTag("InputFieldMaxIters").GetComponent<TMP_InputField>();
        inputFieldThreshold = GameObject.FindGameObjectWithTag("InputFieldThreshold").GetComponent<TMP_InputField>();
        inputFieldDegree = GameObject.FindGameObjectWithTag("InputFieldDegree").GetComponent<TMP_InputField>();

        fractalMandelbrot = GameObject.FindGameObjectWithTag("Mandelbrot").GetComponent<Mandelbrot>();
        fractalJulia = GameObject.FindGameObjectWithTag("Julia").GetComponent<Julia>();

        interfaceController = GameObject.FindGameObjectWithTag("InterfaceController").GetComponent<InterfaceController>();

        dropdownAlgorithm.onValueChanged.AddListener( delegate { DropdownValueChanged(dropdownAlgorithm); });
        dropdownColormap.onValueChanged.AddListener( delegate { DropdownValueChanged(dropdownColormap); });
        dropdownFamily.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdownFamily); });
        sliderMaxIters.onValueChanged.AddListener( delegate { SliderValueChanged(sliderMaxIters); });
        sliderThreshold.onValueChanged.AddListener( delegate { SliderValueChanged(sliderThreshold); });
        sliderDegree.onValueChanged.AddListener( delegate { SliderValueChanged(sliderDegree); });
        //inputFieldMaxIters.onValueChanged.AddListener( delegate  { InputFieldValueChanged(inputFieldMaxIters); });

    }

    public void OnSliderEnd(){
        interfaceController.RefreshFractalMandelbrot();
        interfaceController.RefreshFractalJulia();
    }

    private void DropdownValueChanged(TMP_Dropdown dropdown){
        if (string.Equals(dropdown.tag, "DropdownAlgorithm")){
            fractalMandelbrot.fp.algorithm = dropdown.captionText.text.ToString();
            fractalJulia.fp.algorithm = dropdown.captionText.text.ToString();
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
    }

    void Update(){

        if (allowEnterMaxIters && (Input.GetKey (KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))){
            StartCoroutine(OnSubmit(inputFieldMaxIters, previousMaxIters, 10, 1000));
            allowEnterMaxIters = false;
        }else{
            allowEnterMaxIters = inputFieldMaxIters.isFocused;
        }

        if (allowEnterThreshold && (Input.GetKey (KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))){
            StartCoroutine(OnSubmit(inputFieldThreshold, previousThreshold, 2, 100));
            allowEnterThreshold = false;
        }else{
            allowEnterThreshold = inputFieldThreshold.isFocused;
        }

        if (allowEnterDegree && (Input.GetKey (KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))){
            StartCoroutine(OnSubmit(inputFieldDegree, previousDegree, 2, 10));
            allowEnterDegree = false;
        }else{
            allowEnterDegree = inputFieldDegree.isFocused;
        }


    }

     private IEnumerator OnSubmit(TMP_InputField inputField, int previousValue, int min, int max){
        int result;
        bool changed = false;
        bool error = false;
        if (!string.Equals(inputField.text, previousValue + "")){
            changed = true;
        }
        if (inputField.text.Length == 0){
            LogsController.UpdateLogs(new string[] {inputField.name + " is empty!"}, "#FFA600");
            yield return new WaitForSeconds(0.5f);
            inputField.text = previousValue + "";
            error = true;
        }
        if (int.TryParse(inputField.text, out result)){
            if (result >= min && result <= max){
                if (string.Equals(inputField.tag, "InputFieldMaxIters")){
                    fractalMandelbrot.fp.maxIters = fractalJulia.fp.maxIters = result;
                    sliderMaxIters.value = result;
                    previousMaxIters = result;
                }
                if (string.Equals(inputField.tag, "InputFieldThreshold")){
                    fractalMandelbrot.fp.threshold = fractalJulia.fp.threshold = result;
                    sliderThreshold.value = result;
                    previousValue = result;
                }
            }else{
                LogsController.UpdateLogs(new string[] {"You must input a value between " + min + " and " + max +"."}, "#FFA600");
                yield return new WaitForSeconds(0.5f);
                inputField.text = previousValue.ToString("D");
                error = true;
            }
        }else{
            LogsController.UpdateLogs(new string[] {"Error parsing " + inputField.name + ". Cannot parse '" + inputField.text + "' to int."}, "#FFA600");
            yield return new WaitForSeconds(0.5f);
            inputField.text = previousValue.ToString("D");
            error = true;
        }

        if (!error && changed){
            interfaceController.RefreshFractalMandelbrot();
            interfaceController.RefreshFractalJulia();
        }
    }

}
