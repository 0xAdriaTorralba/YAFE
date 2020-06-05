using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MandelbulbController : MonoBehaviour
{
    private Camera mandelbulbCamera;
    private Slider sliderDegree; 

    private TextMeshProUGUI textDegree;

    private RaymarchController raymarching;
    private RawImage rawImage;

    private TMP_Dropdown dropdownType, dropdownResolution, dropdownDetail;

    void Awake(){
        mandelbulbCamera = GetComponent<Camera>();
        sliderDegree = GameObject.FindGameObjectWithTag("SliderDegree").GetComponent<Slider>();
        dropdownType = GameObject.Find("Type Dropdown").GetComponent<TMP_Dropdown>();
        dropdownResolution = GameObject.Find("Resolution Dropdown").GetComponent<TMP_Dropdown>();
        dropdownDetail = GameObject.Find("Detail Dropdown").GetComponent<TMP_Dropdown>();
        textDegree = GameObject.Find("Degree Text").GetComponent<TextMeshProUGUI>();
        raymarching = GetComponent<RaymarchController>();
        rawImage = GameObject.FindGameObjectWithTag("Mandelbulb").GetComponent<RawImage>();
    }

    void Start()
    {
        mandelbulbCamera.Render();
        sliderDegree.onValueChanged.AddListener( delegate { SliderValueChanged(sliderDegree); });
        dropdownType.onValueChanged.AddListener( delegate { DropdownValueChanged(dropdownType); });
        dropdownDetail.onValueChanged.AddListener( delegate { DropdownValueChanged(dropdownDetail); });
        dropdownResolution.onValueChanged.AddListener( delegate { DropdownValueChanged(dropdownResolution); });

    }

    private void DropdownValueChanged(TMP_Dropdown dropdown){
        if (string.Equals(dropdown.name, "Type Dropdown")){
            switch(dropdown.captionText.text.ToString()){
                case "Mandelbulb":
                    raymarching.type = 1;
                    break;
                case "Sierpinski":
                    raymarching.type = 2;
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
                    //mandelbulbCamera.targetTexture = 
                    break;
                case "Medium (1280x720)":
                    raymarching.type = 2;
                    break;
                case "Low (960x540)":
                    raymarching.type = 3;
                    break;
                default:
                    raymarching.type = 1;
                    break;
            }
        }

    }

    private void SliderValueChanged(Slider slider){
        if (string.Equals(slider.tag, "SliderDegree")){
            raymarching.power = (int) slider.value;
            textDegree.text = (int) slider.value + "";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
