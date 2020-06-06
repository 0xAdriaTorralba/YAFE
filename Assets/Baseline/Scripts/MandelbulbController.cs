using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MandelbulbController : Fractal
{
    private Camera mandelbulbCamera;
    private Slider sliderDegree, sliderMaxIters, sliderIFSIters; 

    private GameObject ifs, degree;

    public RenderTexture[] textures;

    private TextMeshProUGUI textDegree, textIters, textIFS;

    private RaymarchController raymarching;
    private RawImage rawImage;

    private TMP_Dropdown dropdownType, dropdownResolution, dropdownDetail;

    private SimpleCameraController cameraController;
    private Toggle automaticRotationToggle;


    void Awake(){
        mandelbulbCamera = GetComponent<Camera>();
        cameraController = GetComponent<SimpleCameraController>();
        automaticRotationToggle = GameObject.Find("Automatic Cam Toggle").GetComponent<Toggle>();
        
        sliderDegree = GameObject.FindGameObjectWithTag("SliderDegree").GetComponent<Slider>();
        sliderMaxIters = GameObject.Find("Max Iters Slider").GetComponent<Slider>();
        sliderIFSIters = GameObject.Find("IFS Slider").GetComponent<Slider>();
        
        dropdownType = GameObject.Find("Type Dropdown").GetComponent<TMP_Dropdown>();
        dropdownResolution = GameObject.Find("Resolution Dropdown").GetComponent<TMP_Dropdown>();
        dropdownDetail = GameObject.Find("Detail Dropdown").GetComponent<TMP_Dropdown>();
        
        
        textDegree = GameObject.Find("Degree Label Text").GetComponent<TextMeshProUGUI>();
        textIters = GameObject.Find("Max Iters Label Text").GetComponent<TextMeshProUGUI>();
        textIFS = GameObject.Find("IFS Label Text").GetComponent<TextMeshProUGUI>();
        
        raymarching = GetComponent<RaymarchController>();
        rawImage = GameObject.FindGameObjectWithTag("Mandelbulb").GetComponent<RawImage>();

        ifs = GameObject.Find("IFS");
        degree = GameObject.Find("Degree");
    }

    void Start()
    {
        mandelbulbCamera.Render();
        sliderDegree.onValueChanged.AddListener( delegate { SliderValueChanged(sliderDegree); });
        sliderMaxIters.onValueChanged.AddListener( delegate { SliderValueChanged(sliderMaxIters); });
        sliderIFSIters.onValueChanged.AddListener( delegate { SliderValueChanged(sliderIFSIters); });


        dropdownType.onValueChanged.AddListener( delegate { DropdownValueChanged(dropdownType); });
        dropdownDetail.onValueChanged.AddListener( delegate { DropdownValueChanged(dropdownDetail); });
        dropdownResolution.onValueChanged.AddListener( delegate { DropdownValueChanged(dropdownResolution); });

        TogleControls(false, true);
        cameraController.SetAutomaticRotation(true);
        automaticRotationToggle.onValueChanged.AddListener((value) => ToggleParallel(value));

    }

    private void ToggleParallel(bool value){
        automaticRotationToggle.isOn = value;
        cameraController.SetAutomaticRotation(automaticRotationToggle.isOn);

    }

    private void DropdownValueChanged(TMP_Dropdown dropdown){
        if (string.Equals(dropdown.name, "Type Dropdown")){
            switch(dropdown.captionText.text.ToString()){
                case "Mandelbulb":
                    raymarching.type = 1;
                    TogleControls(false, true);
                    break;
                case "Sierpinski":
                    raymarching.type = 2;
                    TogleControls(true, false);
                    break;
                case "Menger Sponge":
                    raymarching.type = 3;
                    break;
                default:
                    raymarching.type = 1;
                    break;
            }
        }
        if (string.Equals(dropdown.name, "Resolution Dropdown")){
            switch(dropdown.captionText.text.ToString()){
                case "High (1920x1080)":
                    mandelbulbCamera.targetTexture = textures[3];
                    rawImage.texture = textures[3];
                    break;
                case "Medium (1280x720)":
                    mandelbulbCamera.targetTexture = textures[2];
                    rawImage.texture = textures[2];
                    break;
                case "Low (960x540)":                    
                    mandelbulbCamera.targetTexture = textures[1];
                    rawImage.texture = textures[1];
                    break;
                case "Very Low (640x360)":
                    mandelbulbCamera.targetTexture = textures[0];
                    rawImage.texture = textures[0]; 
                    break;
                default:
                    mandelbulbCamera.targetTexture = textures[0];
                    rawImage.texture = textures[0];
                    break;
            }
        }
        if (string.Equals(dropdown.name, "Detail Dropdown")){
            switch(dropdown.captionText.text.ToString()){
                case "Ultra":
                    raymarching._RaymarchDrawDistance = 10000;
                    break;
                case "High":
                    raymarching._RaymarchDrawDistance = 5000;
                    break;
                case "Medium":
                    raymarching._RaymarchDrawDistance = 2500;

                    break;
                case "Low":                 
                    raymarching._RaymarchDrawDistance = 1000;
                    break;
                default:
                    raymarching._RaymarchDrawDistance = 1000;
                    break;
            }
        }

    }

    private void SliderValueChanged(Slider slider){
        if (string.Equals(slider.tag, "SliderDegree")){
            raymarching.power = (int) slider.value;
            textDegree.text = (int) slider.value + "";
        }

        if (string.Equals(slider.name, "Max Iters Slider")){
            raymarching.maxsteps = (int) slider.value;
            textIters.text = (int) slider.value + "";
        }
        if (string.Equals(slider.name, "IFS Slider")){
            raymarching.IFSIters = (int) slider.value;
            textIFS.text = (int) slider.value + "";
        }
    }

    private void TogleControls(bool bIFS, bool bDegree){
        ifs.SetActive(bIFS);
        degree.SetActive(bDegree);
    }
}
