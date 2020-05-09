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

    private TMP_Dropdown dropdownAlgorithm, dropdownColormap;
    private Slider sliderMaxIters;
    
    private int previousMaxIters;

    private TMP_InputField inputFieldMaxIters;

    private bool allowEnter = false;
    
    // Start is called before the first frame update
    void Start()
    {
        dropdownAlgorithm = GameObject.FindGameObjectWithTag("DropdownAlgorithm").GetComponent<TMP_Dropdown>();
        dropdownColormap = GameObject.FindGameObjectWithTag("DropdownColormap").GetComponent<TMP_Dropdown>();
        sliderMaxIters = GameObject.FindGameObjectWithTag("SliderMaxIters").GetComponent<Slider>();
        inputFieldMaxIters = GameObject.FindGameObjectWithTag("InputFieldMaxIters").GetComponent<TMP_InputField>();

        fractalMandelbrot = GameObject.FindGameObjectWithTag("Mandelbrot").GetComponent<Mandelbrot>();
        fractalJulia = GameObject.FindGameObjectWithTag("Julia").GetComponent<Julia>();

        interfaceController = GameObject.FindGameObjectWithTag("InterfaceController").GetComponent<InterfaceController>();

        dropdownAlgorithm.onValueChanged.AddListener( delegate { DropdownValueChanged(dropdownAlgorithm); });
        dropdownColormap.onValueChanged.AddListener( delegate { DropdownValueChanged(dropdownColormap); });
        sliderMaxIters.onValueChanged.AddListener( delegate { SliderValueChanged(sliderMaxIters); });
        //inputFieldMaxIters.onValueChanged.AddListener( delegate  { InputFieldValueChanged(inputFieldMaxIters); });

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
    }

    private void SliderValueChanged(Slider slider){
        fractalMandelbrot.fp.maxIters = (int) slider.value;
        fractalJulia.fp.maxIters = (int) slider.value;
        inputFieldMaxIters.text = slider.value + "";
        interfaceController.RefreshFractalMandelbrot();
        interfaceController.RefreshFractalJulia();
    }

    void Update(){

        if (allowEnter && (Input.GetKey (KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))){
            StartCoroutine(OnSubmit());
            allowEnter = false;
        }else{
            allowEnter = inputFieldMaxIters.isFocused;
        }
    }

    private IEnumerator OnSubmit(){
        int result;
        bool changed = false;
        bool error = false;
        if (!string.Equals(inputFieldMaxIters.text, previousMaxIters + "")){
            changed = true;
        }
        if (inputFieldMaxIters.text.Length == 0){
            LogsController.UpdateLogs(new string[] {inputFieldMaxIters.name + " is empty!"}, "#FFA600");
            yield return new WaitForSeconds(0.5f);
            inputFieldMaxIters.text = previousMaxIters + "";
            error = true;
        }
        if (int.TryParse(inputFieldMaxIters.text, out result)){
            if (result >= 10 && result <= 1000){
                fractalMandelbrot.fp.maxIters = result;
                fractalJulia.fp.maxIters = result;
                sliderMaxIters.value = result;
                previousMaxIters = result;
            }else{
                LogsController.UpdateLogs(new string[] {"You must input a max iters value between 10 and 1000."}, "#FFA600");
                yield return new WaitForSeconds(0.5f);
                inputFieldMaxIters.text = previousMaxIters.ToString("D");
                error = true;
            }
        }else{
            LogsController.UpdateLogs(new string[] {"Error parsing " + inputFieldMaxIters.name + ". Cannot parse '" + inputFieldMaxIters.text + "' to int."}, "#FFA600");
            yield return new WaitForSeconds(0.5f);
            inputFieldMaxIters.text = previousMaxIters.ToString("D");
            error = true;
        }

        if (!error && changed){
            interfaceController.RefreshFractalMandelbrot();
            interfaceController.RefreshFractalJulia();
        }
    }
    

}
